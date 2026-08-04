using Fletch.Physics.Model.ResourceHandles;

namespace Fletch.Physics.Model.Info.Collisions
{
    internal readonly struct BackendCollisionEvent
    {
        public IColliderHandle A { get; }

        public IColliderHandle B { get; }

        public CollisionEventType EventType { get; }

        internal BackendCollisionEvent(
            IColliderHandle a,
            IColliderHandle b,
            CollisionEventType eventType)
        {
            A = a;
            B = b;
            EventType = eventType;
        }
    }
}