using DomainLayer.ModelSpace;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace DomainLayer
{
    public class ModelRectangle
    {
        private Vector3 normalVector;
        public List<ModelPoint> Points { get; set; }
        public Color Color { get; set; }

        public ModelRectangle(List<ModelPoint> points, Color color, Vector3 normalVector)
        {
            Points = points;
            Color = color;
            this.normalVector = normalVector;
        }

        public ModelPoint this[int index] => Points[index];

        public ModelTriangle[] Split() => new ModelTriangle[] {new FlatTriangle(new List<ModelPoint> { Points[0], Points[1], Points[2] }, Color, normalVector),
                                        new FlatTriangle(new List<ModelPoint> { Points[2], Points[3], Points[0] }, Color, normalVector)};
    }
}
