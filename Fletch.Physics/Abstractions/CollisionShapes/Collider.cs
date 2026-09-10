using Fletch.Physics.Model;
using Fletch.Physics.Model.Info.Collisions;
using Fletch.Physics.Model.ResourceHandles;
using System.Numerics;

namespace Fletch.Physics.Abstractions.CollisionShapes
{
    public abstract class Collider
    {
        public PhysicsMaterial Material { get => material; set => SetFieldAndDirty(ref material, value); }

        public Vector2 Offset { get => offset; set => SetFieldAndDirty(ref offset, value); }

        private PhysicsMaterial material;

        private Vector2 offset;

        internal bool IsDirty { get; private set; }

        internal IColliderHandle ColliderHandle { get; }

        internal IBodyHandle ParentBodyHandle { get; }

        internal Collider(
            IColliderHandle colliderHandle, 
            IBodyHandle parentBodyHandle, 
            PhysicsMaterial material = new(), 
            Vector2 offset = default)
        {
            ColliderHandle = colliderHandle;
            ParentBodyHandle = parentBodyHandle;
            this.material = material;
            this.offset = offset;
        }

        internal void SetFieldAndDirty<T>(ref T field, T value)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            field = value;
            IsDirty = true;
        }
    }
}
