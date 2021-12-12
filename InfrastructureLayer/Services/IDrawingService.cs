using DomainLayer;
using System.Drawing;

namespace InfrastructureLayer.Services
{
    public interface IDrawingService
    {
        void DrawLineBresenham(IFastBitmap bitmap, Color color, Point p1, Point p2);
    }
}