using Fletch.Engine.Components;
using Fletch.Physics.Abstractions.CollisionShapes;
using Fletch.Physics.Model;
using Fletch.Physics.Model.Info.Collisions;
using Fletch.Physics.Model.ResourceHandles;

namespace Fletch.Physics.Components.CollisionShapes
{
    public class CircleCollider : , ICollisionShape
    {
        public float Radius { get; set; }

        public PhysicsMaterial Material { get; set; } = new();

        public bool Initialized { get; private set; }

        public event Action<CollisionEvent>? OnCollision;

        internal IColliderHandle? ColliderHandle { get; private set; }

        internal IBodyHandle? BodyHandle { get; private set; }

        internal void Initialize(
            IColliderHandle colliderHandle,
            IBodyHandle bodyHandle)
        {
            ColliderHandle = colliderHandle;
            BodyHandle = bodyHandle;
            Initialized = true;
        }

        internal void RaiseCollision(CollisionEvent collision)
        {
            OnCollision?.Invoke(collision);
        }
    }
}
