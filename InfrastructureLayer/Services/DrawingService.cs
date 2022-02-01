using DomainLayer;
using DomainLayer.Dto;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    public class DrawingService
    {
        private readonly IlluminationService illuminationService;
        private readonly DrawingParameters parameters;
        public DrawingService(IlluminationService illuminationService, DrawingParameters drawingParameters)
        {
            this.illuminationService = illuminationService;
            parameters = drawingParameters;
        }
        public void ColorTriangles(IFastBitmap bitmap, List<ModelTriangle> triangles, double[,] zbuffer, Matrix4x4 invertedMatrix, Camera camera)
        {
            //Parallel.ForEach(triangles, triangle => ScanLineColoring(bitmap, triangle, zbuffer, invertedMatrix));
            foreach (var triangle in triangles)
            {
                ScanLineColoring(bitmap, triangle, zbuffer, invertedMatrix, camera);
            }
        }
        private void ScanLineColoring(IFastBitmap bitmap, ModelTriangle triangle, double[,] zbuffer, Matrix4x4 invertedMatrix, Camera camera)
        {
            Vector3 color1 = Vector3.Zero, color2 = Vector3.Zero, color3 = Vector3.Zero;
            var shape = triangle.Points.Select(point => ConvertToCanvas(point, bitmap.Width, bitmap.Height)).ToList();
            if(parameters.ShadingMode == DomainLayer.Enum.ShadingMode.Constant)
            {
                var point = triangle.Points[0].Coordinates; //(triangle.Points[0].Coordinates + triangle.Points[1].Coordinates + triangle.Points[2].Coordinates) / 3;
                var modelVector = Vector4.Transform(point, invertedMatrix);
                modelVector /= modelVector.W;
                var modelPoint = new ModelPoint(modelVector.X, modelVector.Y, modelVector.Z);
                color1 = illuminationService.ComputeColor(modelPoint.Coordinates, triangle.GetNormalVectorForPoint(modelPoint), triangle.Color, camera).From255();
            }
            else if (parameters.ShadingMode == DomainLayer.Enum.ShadingMode.Gouraud)
            {
                var point = triangle.Points[0].Coordinates; //(triangle.Points[0].Coordinates + triangle.Points[1].Coordinates + triangle.Points[2].Coordinates) / 3;
                var modelVector = Vector4.Transform(point, invertedMatrix);
                modelVector /= modelVector.W;
                var modelPoint = new ModelPoint(modelVector.X, modelVector.Y, modelVector.Z);
                color1 = illuminationService.ComputeColor(modelPoint.Coordinates, triangle.GetNormalVectorForPoint(modelPoint), triangle.Color, camera).From255();

                point = triangle.Points[1].Coordinates; //(triangle.Points[0].Coordinates + triangle.Points[1].Coordinates + triangle.Points[2].Coordinates) / 3;
                modelVector = Vector4.Transform(point, invertedMatrix);
                modelVector /= modelVector.W;
                modelPoint = new ModelPoint(modelVector.X, modelVector.Y, modelVector.Z);
                color2 = illuminationService.ComputeColor(modelPoint.Coordinates, triangle.GetNormalVectorForPoint(modelPoint), triangle.Color, camera).From255();

                point = triangle.Points[2].Coordinates; //(triangle.Points[0].Coordinates + triangle.Points[1].Coordinates + triangle.Points[2].Coordinates) / 3;
                modelVector = Vector4.Transform(point, invertedMatrix);
                modelVector /= modelVector.W;
                modelPoint = new ModelPoint(modelVector.X, modelVector.Y, modelVector.Z);
                color3 = illuminationService.ComputeColor(modelPoint.Coordinates, triangle.GetNormalVectorForPoint(modelPoint), triangle.Color, camera).From255();
            }
            var P = shape.Select((point, index) => (point.X, point.Y, index)).OrderBy(shape => shape.Y).ToArray();
            List<(int y_max, double x, double m)> AET = new();
            int ymin = (int)P[0].Y;
            int ymax = (int)P[^1].Y;
            int current_index = 0;
            for (int y = ymin; y <= ymax; ++y)
            {
                // For each point that was on the previous line
                while (P[current_index].Y == y - 1)
                {
                    var current_point = P[current_index];
                    // Find adjacent points
                    var previous_point = Array.Find(P, p => p.index == (current_point.index - 1 + P.Length) % P.Length);
                    var next_point = Array.Find(P, p => p.index == (current_point.index + 1 + P.Length) % P.Length);
                    if (previous_point.Y > current_point.Y)
                    {
                        double m = (current_point.Y - previous_point.Y) / (current_point.X - previous_point.X);
                        if (m != 0)
                            AET.Add(((int)previous_point.Y, P[current_index].X + 1 / m, m));
                    }
                    else
                    {
                        AET.RemoveAll(item => item.y_max == current_point.Y);
                    }
                    if (next_point.Y > current_point.Y)
                    {
                        double m = (current_point.Y - next_point.Y) / (current_point.X - next_point.X);
                        if (m != 0)
                            AET.Add(((int)next_point.Y, P[current_index].X + 1 / m, m));
                    }
                    else
                    {
                        AET.RemoveAll(item => item.y_max == current_point.Y);
                    }
                    ++current_index;
                }
                // Sort edges according to x
                AET.Sort((item1, item2) => item1.x.CompareTo(item2.x));
                // Fill pixels between every pair of edges
                for (int i = 0; i < AET.Count; i += 2)
                {
                    for (int x = (int)Math.Round(AET[i].x); x < AET[i + 1].x; ++x)
                    {
                        if (x < 0 || x >= bitmap.Width || y < 0 || y >= bitmap.Height)
                            continue;
                        float z = GetZ(x, y, shape[0], shape[1], shape[2]);

                        if (z >= zbuffer[x, y])
                            continue;
                        else
                            zbuffer[x, y] = z;

                        var color = Color.White;
                        if (parameters.ShadingMode == DomainLayer.Enum.ShadingMode.Constant)
                        {
                            color = color1.To255();
                        }
                        else if (parameters.ShadingMode == DomainLayer.Enum.ShadingMode.Gouraud)
                        {
                            var factors = GetInterpolationFactors(shape, new Vector3(x, y, z));
                            color = (color1 * factors.X + color2 * factors.Y + color3 * factors.Z).To255();
                        }
                        else
                        {
                            var point = ConvertFromCanvas(new Vector3(x, y, z), bitmap.Width, bitmap.Height);
                            var modelVector = Vector4.Transform(point.Coordinates4, invertedMatrix);
                            modelVector /= modelVector.W;
                            var modelPoint = new ModelPoint(modelVector.X, modelVector.Y, modelVector.Z);
                            color = illuminationService.ComputeColor(modelPoint.Coordinates, triangle.GetNormalVectorForPoint(modelPoint), triangle.Color, camera);
                        }

                        bitmap.SetPixel(x, y, color);
                    }
                }
                // Update the x value for each edge
                for (int i = 0; i < AET.Count; ++i)
                {
                    var (y_max, x, m) = AET[i];
                    AET[i] = (y_max, x + 1 / m, m);
                }
            }
        }

        private Vector3 ConvertToCanvas(ModelPoint point, int width, int height)
        {
            int x = (int)Math.Round(width / 2 * (point.X + 1));
            int y = (int)Math.Round(height / 2 * (-point.Y + 1));
            float z = point.Z;
            return new Vector3(x, y, z);
        }

        private ModelPoint ConvertFromCanvas(Vector3 point, int width, int height)
        {
            float x = 2 * point.X / width - 1;
            float y = -(2 * point.Y / height - 1);
            float z = point.Z;
            return new ModelPoint(x, y, z);
        }

        private static Vector3 GetInterpolationFactors(List<Vector3> shape, Vector3 point)
        {
            var f1 = shape[0] - point;
            var f2 = shape[1] - point;
            var f3 = shape[2] - point;
            var TriangleArea = Vector3.Cross(shape[0] - shape[1], shape[0] - shape[2]).Length();
            var a1 = Vector3.Cross(f2, f3).Length() / TriangleArea;
            var a2 = Vector3.Cross(f3, f1).Length() / TriangleArea;
            var a3 = Vector3.Cross(f1, f2).Length() / TriangleArea;
            return new Vector3(a1, a2, a3);
        }

        /// <summary>
        /// Calculate the third coordinate of a point, given three points of the plane it is on
        /// </summary>
        public static float GetZ(int x, int y, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float z1 = p1.Z * (x - p2.X) * (y - p3.Y) + p2.Z * (x - p3.X) * (y - p1.Y) + p3.Z * (x - p1.X) * (y - p2.Y) - p1.Z * (x - p3.X) * (y - p2.Y) - p2.Z * (x - p1.X) * (y - p3.Y) - p3.Z * (x - p2.X) * (y - p1.Y);
            float z2 = (x - p1.X) * (y - p2.Y) + (x - p2.X) * (y - p3.Y) + (x - p3.X) * (y - p1.Y) - (x - p1.X) * (y - p3.Y) - (x - p2.X) * (y - p1.Y) - (x - p3.X) * (y - p2.Y);

            return z1 / z2;
        }
    }
}
