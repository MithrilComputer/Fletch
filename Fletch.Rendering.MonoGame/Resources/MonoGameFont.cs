using Fletch.Rendering.Abstractions.Resources;
using FontStashSharp;
using System.Numerics;
using XnaVector = Microsoft.Xna.Framework.Vector2;

namespace Fletch.Rendering.MonoGame.Resources
{
    internal class MonoGameFont : IFont
    {
        public SpriteFontBase SpriteFontBase { get; }

        public string? Name { get; }

        public float LineSpacing { get; }

        public MonoGameFont(SpriteFontBase spriteFont, string? name = null)
        {
            SpriteFontBase = spriteFont ?? throw new ArgumentNullException(nameof(spriteFont));
            Name = name ?? spriteFont.ToString();
        }

        public Vector2 MeasureString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Vector2.Zero;

            XnaVector size = SpriteFontBase.MeasureString(text);
            return new Vector2(size.X, size.Y);
        }
    }
}
