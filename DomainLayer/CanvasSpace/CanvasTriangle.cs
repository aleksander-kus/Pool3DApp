using System.Collections.Generic;
using System.Drawing;

namespace DomainLayer
{
    public class CanvasTriangle
    {
        public List<ModelPoint> Points { get; set; }
        public Color Color { get; set; }

        public CanvasTriangle(List<ModelPoint> points, Color color)
        {
            Points = points;
            Color = color;
        }
    }
}
