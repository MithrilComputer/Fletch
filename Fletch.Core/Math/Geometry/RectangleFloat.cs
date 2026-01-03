namespace Fletch.Core.Math.Geometry
{
    public readonly struct RectangleFloat
    {
        public float X { get; }
        public float Y { get; }
        public float Width { get; }
        public float Height { get; }

        public RectangleFloat(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public float Left => X;
        public float Top => Y;
        public float Right => X + Width;
        public float Bottom => Y + Height;

        public bool Contains(float x, float y) =>
            x >= Left && x <= Right &&
            y >= Top && y <= Bottom;

        public bool Intersects(RectangleFloat other) =>
            !(other.Left > Right ||
              other.Right < Left ||
              other.Top > Bottom ||
              other.Bottom < Top);
    }
}
