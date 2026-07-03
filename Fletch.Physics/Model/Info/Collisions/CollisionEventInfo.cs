using Fletch.Physics.Components.CollisionShapes;
using System.Numerics;

namespace Fletch.Physics.Model.Info.Collisions
{
    public readonly struct CollisionEventInfo
    {
        public CollisionShape Self { get; }

        public CollisionShape Other { get; }


        public Vector2 Normal { get; }

        public Vector2 ContactPoint { get; }

        internal CollisionEventInfo(
            CollisionShape self,
            CollisionShape other,
            Vector2 normal,
            Vector2 contactPoint)
        {
            Self = self;
            Other = other;
            Normal = normal;
            ContactPoint = contactPoint;
        }
    }
}