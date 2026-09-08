using Fletch.Physics.Abstractions.CollisionShapes;
using Fletch.Physics.Model.ResourceHandles;

namespace Fletch.Physics.Pools
{
    internal class ColliderHandlePool
    {
        private readonly Dictionary<IColliderHandle, CollisionShape> colliderHandles = new Dictionary<IColliderHandle, CollisionShape>();

        public CollisionShape? GetCollider(IColliderHandle handle)
        {
            if (colliderHandles.TryGetValue(handle, out CollisionShape collider))
            {
                return collider;
            }
            return null;
        }

        public void AddColliderHandle(IColliderHandle handle, CollisionShape collider)
        {
            colliderHandles[handle] = collider;
        }

        public void RemoveColliderHandle(IColliderHandle handle)
        {
            colliderHandles.Remove(handle);
        }
    }
}
