using Fletch.Core.Colors;
using Fletch.Core.Math.Geometry;
using Fletch.Engine.Attributes;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy.Objects;
using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.Model;
using System.Numerics;

namespace Fletch.Rendering.Components
{
    [DisallowMultipleComponentAttribute]
    public class WorldTextRenderer : GameObjectComponent, IInternalWorldRenderingComponent
    {
        public IFont? Font { get; set; }

        public RenderPose RenderPose { get; private set; }

        public WorldTextRenderer(GameObject gameObject) : base(gameObject)
        {
        }

        void IInternalWorldRenderingComponent.SetRenderPose(RenderPose pose)
        {
            RenderPose = pose;
        }

        /// <summary>
        /// Text displayed by the renderer.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Font size expressed in world units.
        /// </summary>
        public float FontSize
        {
            get => fontSize;
            set
            {
                if (!float.IsFinite(value))
                    throw new ArgumentOutOfRangeException(nameof(value));

                fontSize = MathF.Abs(value);
            }
        }

        private float fontSize = 1f;

        /// <summary>
        /// Untransformed dimensions of the text in world units.
        /// </summary>
        public Vector2 LocalSize
        {
            get
            {
                if (Font == null || string.IsNullOrEmpty(Text))
                    return Vector2.Zero;

                return Font.MeasureString(Text) / Font.NominalSize * FontSize;
            }
        }

        /// <summary>
        /// Tint color applied to the text.
        /// </summary>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Draw order relative to other world visuals, Lower renders first.
        /// </summary>
        public byte ZHeight { get; set; } = 0;

        /// <summary>
        /// Internal use ZIndex for index sorting.
        /// </summary>
        internal int ZIndex { get; set; } = 0;

        /// <summary>
        /// Local position offset applied to the text, relative to the entity transform.
        /// </summary>
        public Vector2 PositionOffset { get; set; } = Vector2.Zero;

        /// <summary>
        /// Local scale multiplier applied to the text.
        /// </summary>
        public Vector2 ScaleOffset { get; set; } = Vector2.One;

        /// <summary>
        /// The Rotation Object That Governs the text's rotation offset.
        /// </summary>
        public Rotation RotationOffset { get; set; } = new Rotation();

        /// <summary>
        /// Rotational Origin
        /// </summary>
        public Vector2 Origin { get; set; } = new Vector2(0.5f, 0.5f);
    }
}
