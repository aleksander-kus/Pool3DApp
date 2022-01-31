using System.Numerics;

namespace DomainLayer
{
    public abstract class Camera
    {
        public virtual ModelPoint Position { get; protected set; }
        public virtual ModelPoint Target { get; protected set; }
        public virtual Vector3 UpVector { get; protected set; }

        public Camera(ModelPoint position, ModelPoint target, Vector3 upVector)
        {
            Position = position;
            Target = target;
            UpVector = upVector;
        }
    }
}
