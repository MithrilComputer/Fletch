namespace Fletch.Rendering.Model
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
        public static readonly Color Transparent = new Color(0, 0, 0, 0);
        public static readonly Color Red = new Color(255, 0, 0, 255);
        public static readonly Color Green = new Color(0, 255, 0, 255);
        public static readonly Color Blue = new Color(0, 0, 255, 255);
    }
}
