using Fletch.Core.Math.Geometry;
using Fletch.Engine.Components;
using Fletch.Rendering.Model;
using System.Numerics;

namespace Fletch.Rendering.Components
{
    public class Camera2D : Component
    {
        public Vector2 Position { get; set; }

        public int RenderOrder { get; set; }

        internal int RenderIndex { get; set; }

        public bool IsMainCamera { get; set; }

        public RectangleInt Viewport { get; set; }

        public BlendMode BlendMode { get; set; }

        public SamplerMode SamplerMode { get; set; }
    }
}
