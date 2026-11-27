using System.Numerics;

namespace Fletch.Core.Math.Geometry
{
    internal static class Rectangles
    {
        public static RectangleFloat CalculateAABB(
        Vector2 position,
        Vector2 size,
        Vector2 origin,
        float rotation)
        {
            Vector2 centerOffset = size * (new Vector2(0.5f) - origin);
            Vector2 halfSize = Vector2.Abs(size) * 0.5f;

            float cos = MathF.Cos(rotation);
            float sin = MathF.Sin(rotation);

            Vector2 center = position + new Vector2(
                centerOffset.X * cos - centerOffset.Y * sin,
                centerOffset.X * sin + centerOffset.Y * cos
            );

            Vector2 extents = new Vector2(
                halfSize.X * MathF.Abs(cos) + halfSize.Y * MathF.Abs(sin),
                halfSize.X * MathF.Abs(sin) + halfSize.Y * MathF.Abs(cos)
            );

            return new RectangleFloat(
                center.X - extents.X,
                center.Y - extents.Y,
                extents.X * 2f,
                extents.Y * 2f
            );
        }
    }
}
