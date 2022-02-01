using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace DomainLayer.ModelSpace
{
    public class SphereTriangle : ModelTriangle
    {
        public ModelPoint SphereCenter { get; set; }
        public SphereTriangle(List<ModelPoint> points, Color color, ModelPoint sphereCenter): base(points, color)
        {
            SphereCenter = sphereCenter;
        }
        public override Vector3 GetNormalVectorForPoint(ModelPoint point)
        {
            var color = Vector3.Normalize(point.Coordinates);
            return color;
        }

        public override ModelTriangle NewFromPoints(List<ModelPoint> points)
        {
            return new SphereTriangle(points, Color, SphereCenter);
        }
    }
}
