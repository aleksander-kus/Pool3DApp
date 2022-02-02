using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace DomainLayer.ModelSpace
{
    public class FlatTriangle : ModelTriangle
    {
        public Vector3 OriginalNormalVector { get; set; }
        public Vector3 RotatedNormalVector { get; set; }
        public FlatTriangle(List<ModelPoint> points, Color color, Vector3 normalVector) : base(points, color)
        {
            OriginalNormalVector = normalVector;
            RotatedNormalVector = normalVector;
        }
        public override Vector3 GetNormalVectorForPoint(ModelPoint point)
        {
            return RotatedNormalVector;
        }

        public override ModelTriangle NewFromPoints(List<ModelPoint> points)
        {
            return new FlatTriangle(points, Color, OriginalNormalVector)
            {
                RotatedNormalVector = RotatedNormalVector
            };
        }
    }
}
