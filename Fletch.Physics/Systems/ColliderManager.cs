using Fletch.Engine.Model;
using Fletch.Physics.Abstractions.Backends;
using Fletch.Physics.Abstractions.CollisionShapes;
using Fletch.Physics.Components;
using Fletch.Physics.Components.CollisionShapes;
using Fletch.Physics.Model.Info.Collisions;
using Fletch.Physics.Model.ResourceHandles;
using Fletch.Physics.Pools;

namespace Fletch.Physics.Systems
{
    internal class ColliderManager
    {
        private readonly TrackedSet<ICollisionShape> colliders = new TrackedSet<ICollisionShape>();

        private readonly ColliderHandlePool colliderHandlePool = new ColliderHandlePool();

        private readonly IWorldHandle worldHandle;

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

            foreach (BackendCollisionEvent collisionEvent in collisionEvents)
            {

            } 

            foreach (ICollisionShape collider in colliders.Items)
            {
                if (!collider.Initialized)
                {
                    RigidBody? rigidBody;

                    
                }
            }
        }

        public void OnColliderCreation(ICollisionShape collider)
        {
            switch(collider)// I need a switch >:(
            {
                case BoxCollider boxCollider:

                    boxCollider.Initialize(physicsBackend.CreateCollider());

                    break;

                case CapsuleCollider sphereCollider:
                    
                    break;

                case CircleCollider circleCollider:
                    
                    break;

                default:
                    throw new NotSupportedException($"Unsupported collider type: {collider.GetType().Name}");
            }
        }

        public void OnColliderDistruction()
        {

        }

        public void AddCollider(ICollisionShape collider)
        {
            colliders.MarkToAdd(collider);
        }

        public void RemoveCollider(ICollisionShape collider)
        {
            colliders.MarkToRemove(collider);
        }
    }
}
