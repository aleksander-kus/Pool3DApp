using System.Numerics;

namespace DomainLayer.Cameras
{
    public class CubeOnTopCamera : Camera
    {
        public override ModelPoint Position { get => new(cube.Center.X, cube.Center.Y, 2); protected set => base.Position = value; }

        private readonly MovingCube cube;
        public CubeOnTopCamera(ModelPoint position, ModelPoint target, Vector3 upVector, MovingCube cube) : base(position, target, upVector)
        {
            this.cube = cube;
        }
    }
}
