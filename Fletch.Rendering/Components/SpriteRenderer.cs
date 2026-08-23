using Fletch.Core.Colors;
using Fletch.Core.Math.Geometry;
using Fletch.Engine.Attributes;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy;
using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.Model;
using System.Numerics;

namespace Fletch.Rendering.Components
{
    /// <summary>
    /// Renders a 2D sprite for an GameObject
    /// </summary>
    [DisallowMultipleComponentAttribute]
    public sealed class SpriteRenderer : GameObjectComponent, IInternalRenderingComponent
    {
        /// <summary>
        /// The current render pose of the sprite renderer.
        /// </summary>
        public RenderPose RenderPose { get; private set; }

        /// <summary>
        /// Sets the render pose for the sprite renderer.
        /// </summary>
        public void SetRenderPose(RenderPose pose)
        {
            RenderPose = pose;
        }

        /// <summary>
        /// Sets the initial render pose for the sprite renderer based on the GameObject's transform.
        /// </summary>
        public SpriteRenderer()
        {
            RenderPose = new RenderPose(Transform.WorldPosition, Transform.WorldRotation.Radians);
        }

        /// <summary>
        /// The sprite or texture to render.
        /// </summary>
        public VisualResource VisualResource { get; set; } = new VisualResource();

        //TODO MAKE IT REPORT DIRTY with Component.NotifyComponentChanged() when modified
        /// <summary>
        /// Optional source rectangle within the sprite texture.
        /// </summary>
        public RectangleFloat? SourceRectangle { get; set; } = null;

        public float PixelsPerUnit { get; set; } = 32f;

        /// <summary>
        /// Tint color applied to the sprite.
        /// </summary>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Draw order relative to other sprites.
        /// Lower renders first.
        /// </summary>
        public int ZHeight { get; set; } = 0;

        /// <summary>
        /// Internal use ZIndex for index sorting.
        /// </summary>
        internal int ZIndex { get; set; } = 0;

        /// <summary>
        /// Local position offset applied to the sprite, relative to the entity transform.
        /// </summary>
        public Vector2 PositionOffset { get; set; } = Vector2.Zero;

        /// <summary>
        /// Local scale multiplier applied to the sprite.
        /// </summary>
        public Vector2 ScaleOffset { get; set; } = Vector2.One;

        /// <summary>
        /// The Rotation Object That Governs the sprite's rotation offset.
        /// </summary>
        public Rotation RotationOffset { get; set; } = new Rotation();

        /// <summary>
        /// Rotational Origin
        /// </summary>
        public Vector2 Origin { get; set; } = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// The current sprite effect applied to the sprite.
        /// </summary>
        public SpriteEffect Effect { get; set; } = SpriteEffect.None;
    }
}
