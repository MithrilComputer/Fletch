using Fletch.Physics.Abstractions.CollisionShapes;
using Fletch.Physics.Components;

namespace Fletch.Physics.Model.Info.Collisions
{
    public class CollisionEvent
    {
        public Collider A { get; }

        public Collider B { get; }

        public CollisionEventType EventType { get; }

        internal CollisionEvent(
            Collider a,
            Collider b,
            CollisionEventType eventType)
        {
            A = a;
            B = b;
            EventType = eventType;
        }
    }
}
