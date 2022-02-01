using DomainLayer.ModelSpace;
using System.Collections.Generic;
using System.Numerics;

namespace DomainLayer
{
    public class ModelSphere
    {
        public List<SphereTriangle> Triangles { get; set; }
        public ModelPoint Center { get; set; } = new ModelPoint(Vector3.Zero);
    }
}
