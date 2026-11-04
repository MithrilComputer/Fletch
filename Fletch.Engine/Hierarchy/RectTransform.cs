using Fletch.Core.Math.Geometry;
using System.Numerics;

namespace Fletch.Engine.Hierarchy
{
    public class RectTransform
    {
        public RectTransform? Parent { get; private set; }

        public List<RectTransform> Children { get; } = new();

        public Vector2 AnchorMin { get; set; } = new(0.5f);
        public Vector2 AnchorMax { get; set; } = new(0.5f);

        public Vector2 Pivot { get; set; } = new(0.5f);

        public Vector2 AnchoredPosition { get; set; }

        public Vector2 SizeDelta { get; set; } = new(100, 100);

        public Vector2 CalculatedPosition { get; internal set; }
        public Vector2 CalculatedSize { get; internal set; }

        public Rotation LocalRotation { get; private set; }

        public Rotation WorldRotation => Parent == null ? LocalRotation : new Rotation(Parent.WorldRotation.Degrees + LocalRotation.Degrees);

        public RectTransform(RectTransform? parent = null)
        {
            SetParent(parent);
        }

        public void SetParent(RectTransform? newParent)
        {
            if (Parent == newParent)
                return;

            if (newParent == this)
                throw new InvalidOperationException(
                    "A RectTransform cannot be parented to itself.");

            for (RectTransform? transform = newParent; transform != null; transform = transform.Parent)
            {
                if (transform == this)
                    throw new InvalidOperationException(
                        "Cannot parent a RectTransform to its own descendant.");
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
