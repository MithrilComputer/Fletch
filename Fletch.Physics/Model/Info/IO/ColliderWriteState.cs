using Fletch.Physics.Model.Info.Shapes;

namespace Fletch.Physics.Model.Info.IO
{
    internal struct ColliderWriteState
    {
        public PhysicsMaterial Material { get; }

        public ShapeData Shape { get; }

        public bool IsSensor { get; }

        public bool IsEnabled { get; }

        public ColliderWriteState(
            PhysicsMaterial material,
            ShapeData shape,
            bool isSensor,
            bool isEnabled)
        {
            Material = material;
            Shape = shape;
            IsSensor = isSensor;
            IsEnabled = isEnabled;
        }
    }
}
