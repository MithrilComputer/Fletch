using Fletch.Rendering.Abstractions.Resources;
using Microsoft.Xna.Framework.Graphics;
using System.Numerics;
using XnaVector = Microsoft.Xna.Framework.Vector2;

namespace Fletch.Rendering.MonoGame.Resources
{
    internal class MonoGameFont : IFont
    {
        public SpriteFont SpriteFont { get; }

        public string? Name { get; }

        public float LineSpacing { get; }

        public MonoGameFont(SpriteFont spriteFont, string? name = null)
        {
            SpriteFont = spriteFont ?? throw new ArgumentNullException(nameof(spriteFont));
            Name = name ?? spriteFont.ToString();
        }

        public Vector2 MeasureString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Vector2.Zero;

            XnaVector size = SpriteFont.MeasureString(text);
            return new Vector2(size.X, size.Y);
        }
    }
}
