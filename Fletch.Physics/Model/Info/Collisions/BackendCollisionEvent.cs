using Fletch.Physics.Model.ResourceHandles;

namespace Fletch.Physics.Model.Info.Collisions
{
    internal readonly struct BackendCollisionEvent
    {
        public IColliderHandle A { get; }

        public IColliderHandle B { get; }

        public CollisionEventType EventType { get; }

        /// <summary>
        /// If true, then collider A is always the trigger, B is always the non-trigger.
        /// </summary>
        public bool IsTrigger { get; }

        internal BackendCollisionEvent(
            IColliderHandle a,
            IColliderHandle b,
            CollisionEventType eventType,
            bool isTrigger)
        {
            A = a;
            B = b;
            EventType = eventType;
        }
    }
}