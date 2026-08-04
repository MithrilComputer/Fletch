using Fletch.Core.Math.Geometry;
using System.Numerics;

namespace Fletch.Engine.Hierarchy
{
    public class Transform
    {
        //TODO Add a dirt bool for others to use for updating.

        public Transform? Parent { get; private set; }
        public List<Transform> Children { get; } = new();

        public Vector2 LocalPosition { get; set; } = Vector2.Zero;
        public Vector2 LocalScale { get; set; } = Vector2.One;
        public Rotation LocalRotation { get; private set; }

        public Vector2 WorldPosition => Parent == null ? LocalPosition : Vector2.Transform(LocalPosition, Parent.WorldMatrix);
        public Vector2 WorldScale => Parent == null ? LocalScale : Parent.WorldScale * LocalScale;
        public Rotation WorldRotation => Parent == null ? LocalRotation : new Rotation(Parent.WorldRotation.Degrees + LocalRotation.Degrees);

        public Matrix3x2 LocalMatrix =>
            Matrix3x2.CreateScale(LocalScale) *
            Matrix3x2.CreateRotation(LocalRotation.Radians) *
            Matrix3x2.CreateTranslation(LocalPosition);

        public Matrix3x2 WorldMatrix =>
            Parent == null ? LocalMatrix : LocalMatrix * Parent.WorldMatrix;

        public Transform(float angleDegrees = 0f, Transform? parent = null)
        {
            LocalRotation = new Rotation(angleDegrees);
            SetParent(parent);
        }

        public Transform(Vector2 position, Vector2 scale, float angleDegrees = 0f, Transform? parent = null)
        {
            LocalPosition = position;
            LocalScale = scale;
            LocalRotation = new Rotation(angleDegrees);
            SetParent(parent);
        }

        public void SetParent(Transform? newParent)
        {
            if (newParent == this)
                throw new InvalidOperationException("A Transform cannot be parented to itself.");

            for (var p = newParent; p != null; p = p.Parent)
            {
                if (p == this)
                    throw new InvalidOperationException("Cannot parent a Transform to its own descendant.");
            }

            Parent?.Children.Remove(this);
            Parent = newParent;
            Parent?.Children.Add(this);
        }

        public void SetLocalRotation(float angleDegrees)
        {
            LocalRotation = new Rotation(angleDegrees);
        }
    }
}
