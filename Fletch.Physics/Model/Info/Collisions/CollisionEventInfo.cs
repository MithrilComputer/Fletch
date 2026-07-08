using Fletch.Physics.Components.CollisionShapes;
using System.Numerics;

namespace Fletch.Physics.Model.Info.Collisions
{
    internal readonly struct CollisionEventInfo
    {
        public CollisionShape Self { get; }

        public CollisionShape Other { get; }

        public CollisionEventType EventType { get; }


        public Vector2 Normal { get; }

        public Vector2 ContactPoint { get; }

        internal CollisionEventInfo(
            CollisionShape self,
            CollisionShape other,
            CollisionEventType eventType,
            Vector2 normal,
            Vector2 contactPoint)
        {
            Self = self;
            Other = other;
            EventType = eventType;
            Normal = normal;
            ContactPoint = contactPoint;
        }
    }
}