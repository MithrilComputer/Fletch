using Fletch.Core.Math.Geometry;
using Fletch.Engine.Attributes;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy;
using Fletch.Rendering.Model;
using System.Numerics;

namespace Fletch.Rendering.Components
{
    [DisallowMultipleComponentAttribute]
    public class Camera2D : Component
    {
        public Vector2 Position => GameObject.Transform.WorldPosition;

        public float Zoom { get; set; } = 1;

        public int RenderOrder { get; set; } = 0;

        internal int RenderIndex { get; set; } = 0;

        public bool IsMainCamera { get; set; } = true;

        public RectangleInt Viewport { get; set; } = RectangleInt.Zero;

        public BlendMode BlendMode { get; set; } = BlendMode.Alpha;

        public SamplerMode SamplerMode { get; set; } = SamplerMode.Linear;
    }
}
