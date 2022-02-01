using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace DomainLayer.ModelSpace
{
    public class FlatTriangle : ModelTriangle
    {
        private readonly Vector3 normalVector;
        public FlatTriangle(List<ModelPoint> points, Color color, Vector3 normalVector) : base(points, color)
        {
            this.normalVector = normalVector;
        }
        public override Vector3 GetNormalVectorForPoint(ModelPoint point)
        {
            return normalVector;
        }
    }
}
