using Fletch.Rendering.Model;
using System.Numerics;

namespace Fletch.Rendering.Abstractions.Drawing
{
    /// <summary>
    /// Abstraction for a debug renderer that can draw simple primitives
    /// such as lines, rectangles, circles, paths, and crosses.
    /// </summary>
    public interface IDebugRenderer
    {
        /// <summary>
        /// Enables or disables debug rendering globally.
        /// When disabled, all draw calls are ignored.
        /// </summary>
        bool IsEnabled { get; set; }

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
        void DrawLine(Vector2 start, Vector2 end, float lineThickness, Color color);

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
        void DrawRectangle(Vector2 position, Vector2 size, Vector2 pivot, float rotation, float lineThickness, Color color);

        /// <summary>
        /// Draws the outline of an axis-aligned rectangle without rotation.
        /// </summary>
        /// <param name="position">Center position of the rectangle.</param>
        /// <param name="size">Width and height of the rectangle.</param>
        /// <param name="lineThickness">Thickness of each rectangle edge.</param>
        /// <param name="color">Color used to render the rectangle outline.</param>
        void DrawRectangle(Vector2 position, Vector2 size, float lineThickness, Color color);


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
        void DrawCircle(Vector2 position, float radius, float lineThickness, Color color);

        /// <summary>
        /// Draws a connected polyline going through the given points in order.
        /// </summary>
        /// <param name="pathPoints">Array of points that define the path.</param>
        /// <param name="lineThickness">Thickness of each segment.</param>
        /// <param name="color">Color of the rendered path.</param>
        /// <remarks>
        /// Requires at least two points. Null or too-short paths are ignored.
        /// </remarks>
        void DrawPath(Vector2[] pathPoints, float lineThickness, Color color);

        /// <summary>
        /// Draws a crosshair shaped like a plus sign (+) centered at the given position.
        /// </summary>
        /// <param name="position">Center point of the crosshair.</param>
        /// <param name="size">Total width and height of the crosshair.</param>
        /// <param name="lineThickness">Thickness of the crosshair lines.</param>
        /// <param name="color">Color used to draw the crosshair.</param>
        void DrawCrosshair(Vector2 position, float size, float lineThickness, Color color);

        /// <summary>
        /// Draws an X-shaped cross centered at the given position.
        /// </summary>
        /// <param name="position">Center point of the cross.</param>
        /// <param name="size">Total width and height of the cross.</param>
        /// <param name="lineThickness">Thickness of each diagonal line.</param>
        /// <param name="color">Color used to draw the cross.</param>
        void DrawCross(Vector2 position, float size, float lineThickness, Color color);
    }
}
