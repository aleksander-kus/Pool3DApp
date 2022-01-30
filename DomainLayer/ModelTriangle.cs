using System.Collections.Generic;
using System.Drawing;

namespace DomainLayer
{
    public class ModelTriangle
    {
        public List<ModelPoint> Points { get; set; }
        public Color Color { get; set; }

        public ModelTriangle(List<ModelPoint> points, Color color)
        {
            Points = points;
            Color = color;
        }
    }
}
