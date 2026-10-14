using Fletch.Physics.Model.ResourceHandles;
using System.Numerics;

namespace Fletch.Physics.Model.Forces
{
    internal struct ForceCommand
    {
        public Vector2? VectorForce { get; }

        public Vector2? Point { get; }

        public float AngularForce { get; }

        public ForceType ForceType { get; }

        public IBodyHandle BodyHandle { get; }

        public ForceCommand(IBodyHandle bodyHandle, ForceType forceType, Vector2? vectorForce = null, float angularForce = 0f, Vector2? point = null)
        {
            BodyHandle = bodyHandle;

            ForceType = forceType;

            VectorForce = vectorForce;

            AngularForce = angularForce;

            Point = point;
        }
    }
}
