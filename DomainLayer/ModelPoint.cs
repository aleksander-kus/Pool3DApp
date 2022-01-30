using System.Numerics;

namespace DomainLayer
{
    public struct ModelPoint
    {
        public Vector3 Coordinates { get; set; }

        public ModelPoint(Vector3 coords)
        {
            Coordinates = coords;
        }

        public ModelPoint(float x, float y, float z)
        {
            Coordinates = new Vector3(x, y, z);
        }

        public Vector4 Coordinates4 => new Vector4(Coordinates.X, Coordinates.Y, Coordinates.Z, 1);
    }
}
