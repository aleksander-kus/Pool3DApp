using DomainLayer;
using System;
using System.Drawing;

namespace InfrastructureLayer.Services
{
    public class DrawingService : IDrawingService
    {
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
