using DomainLayer;
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
        public DrawingService(IlluminationService illuminationService)
        {
            this.illuminationService = illuminationService;
        }
        public void DrawContour(IFastBitmap bitmap, List<CanvasRectangle> rectangles, Color color, double [,] zbuffer)
        {
            foreach(var rectangle in rectangles)
            {
                for (int i = 0; i < rectangle.Points.Count; ++i)
                {
                    DrawLineBresenham(bitmap, color, rectangle[i], rectangle[(i + 1) % rectangle.Points.Count], zbuffer);
                }
            }
        }
        public void ColorTriangles(IFastBitmap bitmap, List<ModelTriangle> triangles, double[,] zbuffer, Matrix4x4 invertedMatrix, Camera camera)
        {
            //Parallel.ForEach(triangles, triangle => ScanLineColoring(bitmap, triangle, zbuffer, invertedMatrix));
            foreach (var triangle in triangles)
            {
                ScanLineColoring(bitmap, triangle, zbuffer, invertedMatrix, camera);
            }
            ////triangles.ForEach(triangle => ScanLineColoring(bitmap, triangle, triangle.Color, zbuffer));
            //Random random = new Random(seed);
            //for(int i = 0; i < triangels.Count; i += 2)
            //{
            //    Color color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
            //    ScanLineColoring(bitmap, triangels[i], color, zbuffer);
            //    ScanLineColoring(bitmap, triangels[i+ 1], color, zbuffer);
            //}
            //foreach (var triangle in triangels)
            //{
            //    Color color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
            //    ScanLineColoring(bitmap, triangle, color, zbuffer);
            //}
        }
        private void ScanLineColoring(IFastBitmap bitmap, ModelTriangle triangle, double[,] zbuffer, Matrix4x4 invertedMatrix, Camera camera)
        {
            var shape = triangle.Points.Select(point => ConvertToCanvas(point, bitmap.Width, bitmap.Height)).ToList();
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
                        //lock (zbuffer)
                        //{
                            if (z >= zbuffer[x, y])
                                continue;
                            else
                                zbuffer[x, y] = z;
                        //}
                        var point = ConvertFromCanvas(new Vector3(x, y, z), bitmap.Width, bitmap.Height);
                        var modelVector = Vector4.Transform(point.Coordinates4, invertedMatrix);
                        modelVector /= modelVector.W;
                        var modelPoint = new ModelPoint(modelVector.X, modelVector.Y, modelVector.Z);
                        bitmap.SetPixel(x, y, illuminationService.ComputeColor(modelPoint.Coordinates, triangle.GetNormalVectorForPoint(modelPoint), triangle.Color, camera));
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

        /// <summary>
        /// Calculate the third coordinate of a point, given three points of the plane it is on
        /// </summary>
        public static float GetZ(int x, int y, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float z1 = p1.Z * (x - p2.X) * (y - p3.Y) + p2.Z * (x - p3.X) * (y - p1.Y) + p3.Z * (x - p1.X) * (y - p2.Y) - p1.Z * (x - p3.X) * (y - p2.Y) - p2.Z * (x - p1.X) * (y - p3.Y) - p3.Z * (x - p2.X) * (y - p1.Y);
            float z2 = (x - p1.X) * (y - p2.Y) + (x - p2.X) * (y - p3.Y) + (x - p3.X) * (y - p1.Y) - (x - p1.X) * (y - p3.Y) - (x - p2.X) * (y - p1.Y) - (x - p3.X) * (y - p2.Y);

            return z1 / z2;
        }

        /// <summary>
        /// Draws a between specified points with Bresenham's Algorithm
        /// </summary>
        /// <param name="g"></param>
        /// <param name="pen"></param>
        /// <param name="p1">Starting point</param>
        /// <param name="p2">Ending point</param>
        public static void DrawLineBresenham(IFastBitmap bitmap, Color color, CanvasPoint p1, CanvasPoint p2, double[,] zbuffer)
        {
            using Brush b = new SolidBrush(color);
            int dx = (int)Math.Abs(p2.X - p1.X), dy = (int)Math.Abs(p2.Y - p1.Y);
            int x_increment = (p1.X < p2.X) ? 1 : p1.X == p2.X ? 0 : -1;
            int y_increment = (p1.Y < p2.Y) ? 1 : p1.Y == p2.Y ? 0 : -1;
            // first pixel
            int x = (int)p1.X, y = (int)p1.Y;
            bitmap.SetPixel(x, y, color);
            // go along X-axis
            if (dx > dy)
            {
                int d = 2 * dy - dx;
                int across_increment = (dy - dx) * 2;
                int same_line_increment = 2 * dy;
                float z_delta = (p2.Z - p1.Z) / (p2.X - p1.X);
                if (x_increment == -1)
                    z_delta = -z_delta;
                float z = p1.Z;
                // pętla po kolejnych x
                while (x != p2.X)
                {
                    if (d < 0)  // remain in the same line
                    {
                        d += same_line_increment;
                        x += x_increment;
                    }
                    else  // go across
                    {
                        d += across_increment;
                        x += x_increment;
                        y += y_increment;
                    }
                    z += z_delta;
                    if (x >= 0 && x < zbuffer.GetLength(0) && y >= 0 && y < zbuffer.GetLength(1) && z < zbuffer[x, y])
                    {
                        zbuffer[x, y] = z;
                        bitmap.SetPixel(x, y, color);
                    }

                }
            }
            // go along Y-axis
            else
            {
                int d = 2 * dx - dy;
                int across_increment = (dx - dy) * 2;
                int same_line_increment = 2 * dx;
                float z_delta = (p2.Z - p1.Z) / (p2.Y - p1.Y);
                if (y_increment == -1)
                    z_delta = -z_delta;
                float z = p1.Z;
                while (y != p2.Y)
                {
                    if (d < 0)  // remain in the same line
                    {
                        d += same_line_increment;
                        y += y_increment;
                    }
                    else  // go across
                    {
                        d += across_increment;
                        x += x_increment;
                        y += y_increment;
                    }
                    z += z_delta;
                    if (x >= 0 && x < zbuffer.GetLength(0) && y >= 0 && y < zbuffer.GetLength(1) && z < zbuffer[x, y])
                    {
                        zbuffer[x, y] = z;
                        bitmap.SetPixel(x, y, color);
                    }
                }
            }
        }
    }
}
