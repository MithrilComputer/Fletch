using Fletch.Engine.Model;
using Fletch.Physics.Abstractions.Backends;
using Fletch.Physics.Abstractions.CollisionShapes;
using Fletch.Physics.Components;
using Fletch.Physics.Model.Info.Collisions;
using Fletch.Physics.Model.ResourceHandles;
using System.ComponentModel;

namespace Fletch.Physics.Systems
{
    internal class ColliderManager
    {
        private readonly TrackedSet<CollisionShape> colliders = new TrackedSet<CollisionShape>();

        private IWorldHandle worldHandle;

        private readonly IPhysicsBackend physicsBackend;

        public ColliderManager(IPhysicsBackend physicsBackend, IWorldHandle worldHandle)
        {
            this.physicsBackend = physicsBackend;
            this.worldHandle = worldHandle;
        }

        public void UpdateColliders()
        {
            colliders.Refresh();

            IReadOnlyCollection<BackendCollisionEvent> collisionEvents = physicsBackend.GetCollisionEvents(worldHandle);

            foreach (CollisionShape collider in colliders.Items)
            {
                
            }
        }

        public void AddCollider(CollisionShape collider)
        {
            colliders.MarkToAdd(collider);
        }

        public void RemoveCollider(CollisionShape collider)
        {
            colliders.MarkToRemove(collider);
        }
    }
}
