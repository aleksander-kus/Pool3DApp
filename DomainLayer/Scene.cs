using DomainLayer.ModelSpace;
using System.Collections.Generic;

namespace DomainLayer
{
    public class Scene
    {
        public List<FlatTriangle> TableTriangles { get; set; }
        public MovingCube Cube { get; set; }
        public List<ModelSphere> Spheres { get; set; }
    }
}
