using System.Numerics;

namespace DomainLayer.Cameras
{
    public class CubeFollowingStaticCamera : Camera
    {
        public override ModelPoint Target { get => cube.Center; protected set => base.Target = value; }

        private readonly MovingCube cube;
        public CubeFollowingStaticCamera(ModelPoint position, ModelPoint target, Vector3 upVector, MovingCube cube) : base(position, target, upVector)
        {
            this.cube = cube;
        }
    }
}
