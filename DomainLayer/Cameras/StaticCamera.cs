using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
    public class StaticCamera : Camera
    {
        public StaticCamera(ModelPoint position, ModelPoint target, Vector3 upVector) : base(position, target, upVector)
        {
        }
    }
}
