namespace Fletch.Core.Colors
{
    /// <summary>
    /// Represents a color with red, green, blue, and alpha components.
    /// </summary>
    public struct Color
    {
        /// <summary>
        /// The red component of the color.
        /// </summary>
        public byte R;

        /// <summary>
        /// The green component of the color.
        /// </summary>
        public byte G;

        /// <summary>
        /// The blue component of the color.
        /// </summary>
        public byte B;

        /// <summary>
        /// The alpha (transparency) component of the color.
        /// </summary>
        public byte A;

        public Color(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static readonly Color White = new Color(255, 255, 255, 255);
        public static readonly Color Black = new Color(0, 0, 0, 255);
        public static readonly Color Red = new Color(255, 0, 0, 255);
        public static readonly Color Green = new Color(0, 255, 0, 255);
        public static readonly Color Blue = new Color(0, 0, 255, 255);

        public static readonly Color Yellow = new Color(255, 255, 0, 255);
        public static readonly Color Cyan = new Color(0, 255, 255, 255);
        public static readonly Color Magenta = new Color(255, 0, 255, 255);

        public static readonly Color Gray = new Color(128, 128, 128, 255);
        public static readonly Color LightGray = new Color(211, 211, 211, 255);
        public static readonly Color DarkGray = new Color(64, 64, 64, 255);

        public static readonly Color Orange = new Color(255, 165, 0, 255);
        public static readonly Color Purple = new Color(128, 0, 128, 255);
        public static readonly Color Brown = new Color(165, 42, 42, 255);
        public static readonly Color Pink = new Color(255, 192, 203, 255);

        public static readonly Color CornflowerBlue = new Color(100, 149, 237, 255);
        public static readonly Color SkyBlue = new Color(135, 206, 235, 255);
        public static readonly Color LimeGreen = new Color(50, 205, 50, 255);

        public static readonly Color Transparent = new Color(0, 0, 0, 0);

    }
}
