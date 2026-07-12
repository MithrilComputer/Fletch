using Fletch.Physics.Components.CollisionShapes;
using Fletch.Physics.Model.ResourceHandles;
using System.Numerics;

namespace Fletch.Physics.Model.Info.Collisions
{
    internal readonly struct CollisionEventInfo
    {
        public IColliderHandle A { get; }

        public IColliderHandle B { get; }

        public CollisionEventType EventType { get; }

        internal CollisionEventInfo(
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