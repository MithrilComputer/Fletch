using Fletch.Engine.Components;
using Fletch.Physics.Model;
using Fletch.Physics.Model.Info.Collisions;

namespace Fletch.Physics.Abstractions.CollisionShapes
{
    public abstract class CollisionShape
    {
        internal CollisionShape()
        {
        }

        public PhysicsMaterial Material { get; set; } = new();

        public event Action<Collision>? CollisionEnter;

        public event Action<CollisionEventInfo>? CollisionExit;

        internal void RaiseCollisionEnter(CollisionEventInfo collision)
        {
            CollisionEnter?.Invoke(collision);
        }

        internal void RaiseCollisionExit(CollisionEventInfo collision)
        {
            CollisionExit?.Invoke(collision);
        }
    }
}
