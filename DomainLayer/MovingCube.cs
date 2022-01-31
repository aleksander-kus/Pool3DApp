using System.Collections.Generic;
using System.Numerics;

namespace DomainLayer
{
    public class MovingCube
    {
        public List<ModelRectangle> Walls { get; set; }
        public List<ModelTriangle> Triangles { get; set; }
        public ModelPoint Center { get; set; } = new ModelPoint(Vector3.Zero);
        public int Rotation { get; set; } = 0;
    }
}
