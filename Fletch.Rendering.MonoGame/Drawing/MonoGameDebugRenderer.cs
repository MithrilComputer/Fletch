using Fletch.Rendering.Abstractions.Drawing;
using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.Model;
using SysMatrix = System.Numerics.Matrix3x2;
using SystemVector = System.Numerics.Vector2;

namespace Fletch.Rendering.MonoGame.Drawing
{
    /// <summary>
    /// Simple line based debug renderer built on top of a 1x1 texture and sprite batching.
    /// Intended for visualizing primitives such as lines, rectangles, circles, and paths.
    /// </summary>
    internal sealed class MonoGameDebugRenderer : IDebugRenderer
    {
        private readonly ISpriteBatcher spriteBatch;
        private readonly ITexture pixelTexture;

        private const int CircleSegments = 64;

        /// <summary>
        /// When true, draw calls are processed. When false, all draw methods early-out.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        private static readonly float[] cosTable = new float[CircleSegments];
        private static readonly float[] sinTable = new float[CircleSegments];

        static MonoGameDebugRenderer()
        {
            for (int i = 0; i < CircleSegments; i++)
            {
                float angle = MathF.PI * 2.0f * i / CircleSegments;
                cosTable[i] = MathF.Cos(angle);
                sinTable[i] = MathF.Sin(angle);
            }
        }

        public MonoGameDebugRenderer(ISpriteBatcher spriteBatch, ITexture pixelTexture)
        {
            this.spriteBatch = spriteBatch;
            this.pixelTexture = pixelTexture;
        }

        /// <summary>
        /// Draws a straight line segment between two points.
        /// </summary>
        /// <param name="start">World position of the start point.</param>
        /// <param name="end">World position of the end point.</param>
        /// <param name="lineThickness">Visual thickness of the line.</param>
        /// <param name="color">Color of the rendered line.</param>
        /// <remarks>
        /// No line is drawn if the length is zero or drawing is disabled.
        /// </remarks>
        public void DrawLine(SystemVector start, SystemVector end, float lineThickness, Color color)
        {
            if (!IsEnabled)
                return;

            SystemVector delta = end - start;

            float length = delta.Length();

            if (length <= float.Epsilon)
                return;

            float rotation = MathF.Atan2(delta.Y, delta.X);

            spriteBatch.Draw(
                texture: pixelTexture,
                position: start,
                sourceRectangle: null,
                color: color,
                rotation: rotation,
                origin: new SystemVector(0, 0.5f),
                scale: new SystemVector(length, lineThickness),
                spriteEffect: SpriteEffect.None,
                layerDepth: 0f);
        }

        /// <summary>
        /// Draws the outline of a rotated rectangle.
        /// </summary>
        /// <param name="position">World-space position of the rectangle.</param>
        /// <param name="size">Width and height of the rectangle.</param>
        /// <param name="pivot">
        /// Pivot point in the rectangle's local space that rotation occurs around.
        /// Local space is centered at (0,0), with range approximately -halfSize..+halfSize.
        /// </param>
        /// <param name="rotation">Rotation in radians around the pivot point.</param>
        /// <param name="lineThickness">Thickness of the rectangle edges.</param>
        /// <param name="color">Color used for rendering the rectangle outline.</param>
        public void DrawRectangle(SystemVector position, SystemVector size, SystemVector pivot, float rotation, float lineThickness, Color color)
        {
            if (!IsEnabled)
                return;

            SystemVector halfSize = size * 0.5f;

            SystemVector topLeftCorner = new SystemVector(-halfSize.X, halfSize.Y);
            SystemVector topRightCorner = new SystemVector(halfSize.X, halfSize.Y);
            SystemVector bottomRightCorner = new SystemVector(halfSize.X, -halfSize.Y);
            SystemVector bottomLeftCorner = new SystemVector(-halfSize.X, -halfSize.Y);

            SysMatrix transform =
                SysMatrix.CreateTranslation(-pivot) *
                SysMatrix.CreateRotation(rotation) *
                SysMatrix.CreateTranslation(position);

            topLeftCorner = SystemVector.Transform(topLeftCorner, transform);
            topRightCorner = SystemVector.Transform(topRightCorner, transform);
            bottomRightCorner = SystemVector.Transform(bottomRightCorner, transform);
            bottomLeftCorner = SystemVector.Transform(bottomLeftCorner, transform);

            DrawLine(topLeftCorner, topRightCorner, lineThickness, color);
            DrawLine(topRightCorner, bottomRightCorner, lineThickness, color);
            DrawLine(bottomRightCorner, bottomLeftCorner, lineThickness, color);
            DrawLine(bottomLeftCorner, topLeftCorner, lineThickness, color);
        }

        /// <summary>
        /// Draws the outline of an axis-aligned rectangle without rotation.
        /// </summary>
        /// <param name="position">Center position of the rectangle.</param>
        /// <param name="size">Width and height of the rectangle.</param>
        /// <param name="lineThickness">Thickness of each rectangle edge.</param>
        /// <param name="color">Color used to render the rectangle outline.</param>
        public void DrawRectangle(SystemVector position, SystemVector size, float lineThickness, Color color)
        {
            if (!IsEnabled)
                return;

            SystemVector halfSize = size * 0.5f;

            SystemVector topLeftCorner = new SystemVector(-halfSize.X, halfSize.Y) + position;
            SystemVector topRightCorner = new SystemVector(halfSize.X, halfSize.Y) + position;
            SystemVector bottomLeftCorner = new SystemVector(-halfSize.X, -halfSize.Y) + position;
            SystemVector bottomRightCorner = new SystemVector(halfSize.X, -halfSize.Y) + position;

            DrawLine(topLeftCorner, topRightCorner, lineThickness, color);
            DrawLine(topRightCorner, bottomRightCorner, lineThickness, color);
            DrawLine(bottomRightCorner, bottomLeftCorner, lineThickness, color);
            DrawLine(bottomLeftCorner, topLeftCorner, lineThickness, color);
        }

        /// <summary>
        /// Draws the outline of a circle using line segments.
        /// </summary>
        /// <param name="position">Center position of the circle.</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="lineThickness">Thickness of each segment.</param>
        /// <param name="color">Color of the circle outline.</param>
        /// <remarks>
        /// The circle is approximated using a fixed number of segments.
        /// No circle is drawn if the radius is non-positive.
        /// </remarks>
        public void DrawCircle(SystemVector position, float radius, float lineThickness, Color color)
        {
            if (!IsEnabled || CircleSegments < 3 || radius <= 0f)
                return;

            for (int i = 0; i < CircleSegments; i++)
            {
                SystemVector start = new SystemVector(
                    position.X + (radius * cosTable[i]),
                    position.Y + (radius * sinTable[i]));

                SystemVector end = new SystemVector(
                    position.X + (radius * cosTable[(i + 1) % CircleSegments]),
                    position.Y + (radius * sinTable[(i + 1) % CircleSegments]));

                DrawLine(start, end, lineThickness, color);
            }
        }

        /// <summary>
        /// Draws a connected polyline going through the given points in order.
        /// </summary>
        /// <param name="pathPoints">Array of points that define the path.</param>
        /// <param name="lineThickness">Thickness of each segment.</param>
        /// <param name="color">Color of the rendered path.</param>
        /// <remarks>
        /// Requires at least two points. Null or too-short paths are ignored.
        /// </remarks>
        public void DrawPath(SystemVector[] pathPoints, float lineThickness, Color color)
        {
            if (!IsEnabled || pathPoints == null || pathPoints.Length < 2)
                return;

            for (int i = 0; i < pathPoints.Length - 1; i++)
            {
                DrawLine(pathPoints[i], pathPoints[i + 1], lineThickness, color);
            }
        }

        /// <summary>
        /// Draws a crosshair shaped like a plus sign (+) centered at the given position.
        /// </summary>
        /// <param name="position">Center point of the crosshair.</param>
        /// <param name="size">Total width and height of the crosshair.</param>
        /// <param name="lineThickness">Thickness of the crosshair lines.</param>
        /// <param name="color">Color used to draw the crosshair.</param>
        public void DrawCrosshair(SystemVector position, float size, float lineThickness, Color color)
        {
            if (!IsEnabled)
                return;

            float halfSize = size * 0.5f;

            SystemVector horizontalStart = new SystemVector(position.X - halfSize, position.Y);
            SystemVector horizontalEnd = new SystemVector(position.X + halfSize, position.Y);

            SystemVector verticalStart = new SystemVector(position.X, position.Y - halfSize);
            SystemVector verticalEnd = new SystemVector(position.X, position.Y + halfSize);

            DrawLine(horizontalStart, horizontalEnd, lineThickness, color);
            DrawLine(verticalStart, verticalEnd, lineThickness, color);
        }

        /// <summary>
        /// Draws an X-shaped cross centered at the given position.
        /// </summary>
        /// <param name="position">Center point of the cross.</param>
        /// <param name="size">Total width and height of the cross.</param>
        /// <param name="lineThickness">Thickness of each diagonal line.</param>
        /// <param name="color">Color used to draw the cross.</param>
        public void DrawCross(SystemVector position, float size, float lineThickness, Color color)
        {
            if (!IsEnabled)
                return;

            float halfSize = size * 0.5f;

            SystemVector topLeftCorner = new SystemVector(position.X - halfSize, position.Y + halfSize);
            SystemVector topRightCorner = new SystemVector(position.X + halfSize, position.Y + halfSize);
            SystemVector bottomRightCorner = new SystemVector(position.X + halfSize, position.Y - halfSize);
            SystemVector bottomLeftCorner = new SystemVector(position.X - halfSize, position.Y - halfSize);

            DrawLine(topLeftCorner, bottomRightCorner, lineThickness, color);
            DrawLine(topRightCorner, bottomLeftCorner, lineThickness, color);
        }
    }
}
