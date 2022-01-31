using System.Collections.Generic;
using System.Drawing;

namespace DomainLayer
{
    public class CanvasTriangle
    {
        public List<CanvasPoint> Points { get; set; }
        public Color Color { get; set; }

        public CanvasTriangle(List<CanvasPoint> points, Color color)
        {
            Points = points;
            Color = color;
        }
    }
}
