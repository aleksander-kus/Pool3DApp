using System.Collections.Generic;
using System.Drawing;

namespace DomainLayer
{
    public class ModelRectangle
    {
        public List<ModelPoint> Points { get; set; }
        public Color Color { get; set; }

        public ModelRectangle(List<ModelPoint> points, Color color)
        {
            Points = points;
            Color = color;
        }

        public ModelPoint this[int index] => Points[index];

        public ModelTriangle[] Split() => new ModelTriangle[] {new ModelTriangle(new List<ModelPoint> { Points[0], Points[1], Points[2] }, Color),
                                        new ModelTriangle(new List<ModelPoint> { Points[2], Points[3], Points[0] }, Color)};
    }
}
