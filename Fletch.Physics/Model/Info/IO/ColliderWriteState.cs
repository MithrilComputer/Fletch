using Fletch.Physics.Model.Info.Masking.Collisions;
using Fletch.Physics.Model.Info.Shapes;
using System.Numerics;

namespace Fletch.Physics.Model.Info.IO
{
    [Flags]
    internal enum ColliderDirtyFlags : byte
    {
        None = 0,
        Material = 1,
        Shape = 1 << 1,
        Filter = 1 << 2,
        IsSensor = 1 << 3,

        All = Material | Shape | Filter | IsSensor
    }

    internal struct ColliderWriteState
    {
        public ColliderDirtyFlags DirtyFlags { get; }

        public PhysicsMaterial Material { get; }

        public ShapeData Shape { get; }

        public CollisionFilter Filter { get; }

        public Vector2 Offset { get; }

        public float Rotation { get; }

        public bool IsSensor { get; }

        public ColliderWriteState(
            PhysicsMaterial material,
            ShapeData shape,
            CollisionFilter filter,
            Vector2 offset,
            float rotation,
            bool isSensor)
        {
            Material = material;
            Shape = shape;
            Filter = filter;
            Offset = offset;
            Rotation = rotation;
            IsSensor = isSensor;
        }
    }
}
