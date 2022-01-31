using System.Collections.Generic;
using System.Drawing;

namespace DomainLayer
{
    public class CanvasRectangle
    {
        public List<CanvasPoint> Points { get; set; }
        public Color Color { get; set; }

        public CanvasRectangle(List<CanvasPoint> points, Color color)
        {
            Points = points;
            Color = color;
        }

        public CanvasPoint this[int index] => Points[index];
    }
}
