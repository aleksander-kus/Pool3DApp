using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace DomainLayer.ModelSpace
{
    public class SphereTriangle : ModelTriangle
    {
        private readonly ModelPoint sphereCenter;
        public SphereTriangle(List<ModelPoint> points, Color color, ModelPoint sphereCenter): base(points, color)
        {
            this.sphereCenter = sphereCenter;
        }
        public override Vector3 GetNormalVectorForPoint(ModelPoint point)
        {
            return Vector3.Normalize(point.Coordinates - sphereCenter.Coordinates);
        }
    }
}
