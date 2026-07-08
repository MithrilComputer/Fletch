using Fletch.Physics.Model.Info.Shapes;
using System.Numerics;

namespace Fletch.Physics.Model.Info.IO
{
    [Flags]
    internal enum ColliderDirtyFlags : byte
    {
        Material = 0,
        Shape = 1 << 0,
        Filter = 1 << 1,
        IsSensor = 1 << 2,

        All = Material | Shape | Filter | IsSensor
    }

    internal struct ColliderWriteState
    {
        public ColliderDirtyFlags DirtyFlags { get; }

        public PhysicsMaterial Material { get; }

        public ShapeData Shape { get; }

        public Vector2 Offset { get; }

        public float Rotation { get; }

        public bool IsSensor { get; }

        public ColliderWriteState(
            PhysicsMaterial material,
            ShapeData shape,
            Vector2 offset,
            float rotation,
            bool isSensor)
        {
            Material = material;
            Shape = shape;
            Offset = offset;
            Rotation = rotation;
            IsSensor = isSensor;
        }
    }
}
