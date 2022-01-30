using System.Numerics;

namespace DomainLayer
{
    public struct CanvasPoint
    {
        public Vector3 Coordinates { get; set; }

        public CanvasPoint(Vector3 coords)
        {
            Coordinates = coords;
        }

        public CanvasPoint(float x, float y, float z)
        {
            Coordinates = new Vector3(x, y, z);
        }

        public CanvasPoint(Vector4 coords)
        {
            Coordinates = new Vector3(coords.X, coords.Y, coords.Z);
        }
    }
}
