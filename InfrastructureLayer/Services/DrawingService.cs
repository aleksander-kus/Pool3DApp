using DomainLayer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace InfrastructureLayer.Services
{
    public class DrawingService
    {
        public void DrawContour(IFastBitmap bitmap, List<CanvasRectangle> rectangles, double [,] zbuffer)
        {

        }
        public void ColorTriangles(IFastBitmap bitmap, List<CanvasTriangle> triangels, double[,] zbuffer)
        {
            triangels.ForEach(triangle => ScanLineColoring(bitmap, triangle.Points.Select(point => point.Coordinates).ToList(), triangle.Color, zbuffer));
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
        private void ScanLineColoring(IFastBitmap bitmap, List<Vector3> shape, Color color, double[,] zbuffer)
        {
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
                        double z = GetZ(x, y, shape[0], shape[1], shape[2]);
                        if(z < zbuffer[x, y])
                        {
                            zbuffer[x,y] = z;
                            bitmap.SetPixel(x, y, color);
                        }
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
