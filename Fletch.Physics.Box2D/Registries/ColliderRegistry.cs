using Box2D.NET;
using Fletch.Physics.Box2D.Model.ResourceHandles;

namespace Fletch.Physics.Box2D.Registries
{
    internal class ColliderRegistry
    {
        private static readonly Dictionary<B2ShapeId, Box2DColiderHandle> shapeIdColliderBinding;
        
        static ColliderRegistry()
        {
            shapeIdColliderBinding = new Dictionary<B2ShapeId, Box2DColiderHandle>();
        }

        public bool TryGetColliderHandleFromShapeId(B2ShapeId shapeId, out Box2DColiderHandle? handle)
        {
            if(shapeIdColliderBinding.TryGetValue(shapeId, out Box2DColiderHandle getHandle))
            {
                handle = getHandle;

                return true;
            }

            handle = null;

            return false;
        }

        public void RegisterCollider(Box2DColiderHandle handle)
        {
            if(!shapeIdColliderBinding.ContainsKey(handle.Id))
            {
                shapeIdColliderBinding.Add(handle.Id, handle);
                return;
            }

            throw new InvalidOperationException();
        }

        public void RemoveRegistration(Box2DColiderHandle handle)
        {
            shapeIdColliderBinding.Remove(handle.Id);
        }

        public void RemoveRegistration(B2ShapeId handle)
        {
            shapeIdColliderBinding.Remove(handle);
        }
    }
}
