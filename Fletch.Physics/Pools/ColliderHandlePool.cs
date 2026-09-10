using Fletch.Physics.Abstractions.CollisionShapes;
using Fletch.Physics.Model.ResourceHandles;

namespace Fletch.Physics.Pools
{
    internal class ColliderHandlePool
    {
        private readonly Dictionary<IColliderHandle, ICollisionShape> colliderHandles = new Dictionary<IColliderHandle, ICollisionShape>();

        public ICollisionShape? GetCollider(IColliderHandle handle)
        {
            if (colliderHandles.TryGetValue(handle, out ICollisionShape? collider))
            {
                return collider;
            }
            return null;
        }

        public void AddColliderHandle(IColliderHandle handle, ICollisionShape collider)
        {
            if(!colliderHandles.TryAdd(handle, collider))
            {
                // TODO Log dupe
            }
        }

        public void RemoveColliderHandle(IColliderHandle handle)
        {
            colliderHandles.Remove(handle);
        }
    }
}
