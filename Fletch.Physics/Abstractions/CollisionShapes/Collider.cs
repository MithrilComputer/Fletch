using Fletch.Physics.Model;
using Fletch.Physics.Model.Info.IO;
using Fletch.Physics.Model.ResourceHandles;
using System.Numerics;

namespace Fletch.Physics.Abstractions.CollisionShapes
{
    public abstract class Collider
    {
        public PhysicsMaterial Material { get => material; set => SetFieldAndDirty(ref material, value, ColliderDirtyFlags.Material); }

        public Vector2 Offset { get => offset; set => SetFieldAndDirty(ref offset, value, ColliderDirtyFlags.Shape); }

        public float Mass { get => mass; set => SetFieldAndDirty(ref mass, value, ColliderDirtyFlags.Shape); }

        public bool IsSensor { get => isSensor; set => SetFieldAndDirty(ref isSensor, value, ColliderDirtyFlags.IsSensor); }

        private PhysicsMaterial material;

        private Vector2 offset;

        private float mass;

        private bool isSensor;

        internal bool IsDirty { get; private set; }

        internal IColliderHandle ColliderHandle { get; }

        internal IBodyHandle ParentBodyHandle { get; }

        internal ColliderDirtyFlags DirtyFlags { get; private set; } = ColliderDirtyFlags.None;

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

        internal void SetFieldAndDirty<T>(
            ref T field,
            T value,
            ColliderDirtyFlags flag)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            field = value;
            DirtyFlags |= flag;
        }

        internal void ClearDirty()
        {
            DirtyFlags = ColliderDirtyFlags.None;
        }

        internal void MarkDirty(ColliderDirtyFlags flags)
        {
            DirtyFlags |= flags;
        }
    }
}
