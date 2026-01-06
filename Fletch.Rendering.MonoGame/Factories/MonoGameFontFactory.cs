using Fletch.Rendering.Abstractions.Factories;
using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.MonoGame.Resources;
using FontStashSharp;

namespace Fletch.Rendering.MonoGame.Factories
{
    internal class MonoGameFontFactory : IFontFactory
    {
        private readonly FontSystem fontSystem;

        Dictionary<string, IFont> fontCache = new();

        private readonly Dictionary<string, float> fontSizes = new()
        {
            { "Default", 16f },
            { "Small",   12f },
            { "Title",   32f }
        };

        public MonoGameFontFactory()
        {
            fontSystem = new FontSystem();

            var bytes = File.ReadAllBytes("Assets/Fonts/Arial.ttf");
            fontSystem.AddFont(bytes);
        }

        public IFont GetFont(string fontName)
        {
            if (fontCache.TryGetValue(fontName, out IFont? font))
            {
                return font;
            }

            if (!fontSizes.TryGetValue(fontName, out float size))
                throw new KeyNotFoundException(
                    $"Font '{fontName}' is not registered. Add it to 'fontSizes' in MonoGameFontFactory.");

            SpriteFontBase spriteFont = fontSystem.GetFont(size);

            var wrapped = new MonoGameFont(spriteFont, fontName);
            fontCache[fontName] = wrapped;

            return wrapped;
        }

        public bool TryGetFont(string fontName, out IFont? font)
        {
            if (fontCache.TryGetValue(fontName, out font))
                return true;

            if (!fontSizes.TryGetValue(fontName, out float size))
            {
                font = null;
                return false;
            }

            SpriteFontBase spriteFont = fontSystem.GetFont(size);

            font = new MonoGameFont(spriteFont, fontName);
            fontCache[fontName] = font;

            return true;
        }
    }
}
