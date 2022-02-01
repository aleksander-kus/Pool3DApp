using System.Numerics;

namespace DomainLayer.Cameras
{
    public class CubeOnTopCamera : Camera
    {
        public override ModelPoint Position { get => new(cube.Center.X, cube.Center.Y, 1.5f); protected set => base.Position = value; }
        public override ModelPoint Target { get => new(.5f, 1f, 0); protected set => base.Position = value; }

        private readonly MovingCube cube;
        public CubeOnTopCamera(ModelPoint position, ModelPoint target, Vector3 upVector, MovingCube cube) : base(position, target, upVector)
        {
            this.cube = cube;
        }
    }
}
