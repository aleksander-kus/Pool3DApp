using DomainLayer;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace InfrastructureLayer.Services
{
    public interface IDrawingService
    {
        void DrawLineBresenham(IFastBitmap bitmap, Color color, Point p1, Point p2);
        void ColorTriangles(IFastBitmap bitmap, List<List<Vector3>> triangels, double[,] zbuffer);
    }
}