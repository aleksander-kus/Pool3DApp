using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace DomainLayer
{
    public abstract class ModelTriangle
    {
        public List<ModelPoint> Points { get; set; }
        public Color Color { get; set; }

        public ModelTriangle(List<ModelPoint> points, Color color)
        {
            Points = points;
            Color = color;
        }

        public abstract Vector3 GetNormalVectorForPoint(ModelPoint point);
    }
}
