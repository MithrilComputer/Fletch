using Fletch.Physics.Components;
using Fletch.Physics.Model.ResourceHandles;

namespace Fletch.Physics.Maps
{
    internal class ColliderRigidbodyMap
    {
        private readonly Dictionary<IColliderHandle, RigidBody> rigidBodyMap;

        public ColliderRigidbodyMap()
        {
            rigidBodyMap = new Dictionary<IColliderHandle, RigidBody>();
        }

        public void AddMapping(IColliderHandle colliderHandle, RigidBody rigidBody)
        {
            if(!rigidBodyMap.TryAdd(colliderHandle, rigidBody))
            {
                throw new ArgumentException("A rigid body is already mapped to the specified collider handle.");
            } 
        }

        public void RemoveMapping(IColliderHandle colliderHandle)
        {
            rigidBodyMap.Remove(colliderHandle);
        }

        public RigidBody? GetRigidBody(IColliderHandle colliderHandle)
        {
            rigidBodyMap.TryGetValue(colliderHandle, out RigidBody? rigidBody);
            return rigidBody;
        }
    }
}
