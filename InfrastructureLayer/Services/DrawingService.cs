using DomainLayer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace InfrastructureLayer.Services
{
    public class DrawingService : IDrawingService
    {
        public void ColorTriangles(IFastBitmap bitmap, List<List<Vector3>> triangels, double[,] zbuffer)
        {
            Random random = new Random(1234);
            foreach (var triangle in triangels)
            {
                Color color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
                ScanLineColoring(bitmap, triangle, color, zbuffer);
            }
        }
        private void ScanLineColoring(IFastBitmap bitmap, List<Vector3> shape, Color color, double[,] zbuffer)
        {
            var P = shape.Select((point, index) => (point.X, point.Y, index)).OrderBy(shape => shape.Y).ToArray();
            List<(int y_max, double x, double m)> AET = new();
            int ymin = (int)P[0].Y;
            int ymax = (int)P[^1].Y;
            int current_index = 0;
            //Vector3 color1 = colorService.ComputeColor(shape[0], parameters).From255();
            //Vector3 color2 = colorService.ComputeColor(shape[1], parameters).From255();
            //Vector3 color3 = colorService.ComputeColor(shape[2], parameters).From255();
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
                        //Color color = Color.Black;
                        //float z = GetZ(x, y, shape[0], shape[1], shape[2]);
                        //if (parameters.FillMode == FillMode.Interpolation)
                        //{
                        //    var factors = GetInterpolationFactors(shape, new Vector3(x, y, z), parameters);
                        //    color = (color1 * factors.X + color2 * factors.Y + color3 * factors.Z).To255();
                        //}
                        //else
                        //    color = colorService.ComputeColor(new Vector3(x, y, z), parameters);
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

        public void DrawLineBresenham(IFastBitmap bitmap, Color color, Point p1, Point p2)
        {
            int dx = Math.Abs(p2.X - p1.X), dy = Math.Abs(p2.Y - p1.Y);
            int x_increment = (p1.X < p2.X) ? 1 : p1.X == p2.X ? 0 : -1;
            int y_increment = (p1.Y < p2.Y) ? 1 : p1.Y == p2.Y ? 0 : -1;
            // first pixel
            int x = p1.X, y = p1.Y;
            bitmap.SetPixel(x, y, color);
            // go along X-axis
            if (dx > dy)
            {
                int d = 2 * dy - dx;
                int across_increment = (dy - dx) * 2;
                int same_line_increment = 2 * dy;
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
                    bitmap.SetPixel(x, y, color);
                }
            }
            // go along Y-axis
            else
            {
                int d = 2 * dx - dy;
                int across_increment = (dx - dy) * 2;
                int same_line_increment = 2 * dx;
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
                    bitmap.SetPixel(x, y, color);
                }
            }
        }
    }
}
