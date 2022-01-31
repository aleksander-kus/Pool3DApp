using System.Collections.Generic;

namespace DomainLayer
{
    public class Scene
    {
        public List<ModelTriangle> TableTriangles { get; set; }
        public MovingCube Cube { get; set; }
        public List<ModelSphere> Spheres { get; set; }
    }
}
