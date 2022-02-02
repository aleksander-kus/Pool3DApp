using DomainLayer;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace InfrastructureLayer.Services
{
    public interface IDrawingService
    {
        void ColorTriangles(IFastBitmap bitmap, List<CanvasTriangle> triangles, double[,] zbuffer, int seed);
    }
}