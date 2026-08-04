using Fletch.Engine.Hierarchy;
using Fletch.Physics.Components;

namespace Fletch.Physics.Model.Info.Collisions
{
    public class CollisionEvent
    {
        public RigidBody Self { get; }

        public RigidBody Other { get; }

        public CollisionEventType EventType { get; }

        internal CollisionEvent(
            RigidBody self,
            RigidBody other,
            CollisionEventType eventType)
        {
            Self = self;
            Other = other;
            EventType = eventType;
        }
    }
}
