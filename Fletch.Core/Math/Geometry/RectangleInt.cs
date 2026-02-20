namespace Fletch.Core.Math.Geometry
{
    public readonly struct RectangleInt : IEquatable<RectangleInt>
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public RectangleInt(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public int Left => X;
        public int Top => Y;
        public int Right => X + Width;
        public int Bottom => Y + Height;

        public bool Contains(int x, int y) =>
            x >= Left && x <= Right &&
            y >= Top && y <= Bottom;

        public bool Intersects(RectangleFloat other) =>
            !(other.Left > Right ||
              other.Right < Left ||
              other.Top > Bottom ||
              other.Bottom < Top);

        public bool Equals(RectangleInt other)
        {
            return X == other.X &&
                   Y == other.Y &&
                   Width == other.Width &&
                   Height == other.Height;
        }

        public static readonly RectangleInt Zero = new RectangleInt(0,0,0,0);
    }
}
