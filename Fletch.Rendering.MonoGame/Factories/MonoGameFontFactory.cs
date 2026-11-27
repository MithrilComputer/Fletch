using Fletch.Rendering.Abstractions.Factories;
using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.MonoGame.Resources;
using FontStashSharp;

namespace Fletch.Rendering.MonoGame.Factories
{
    /// <summary>
    /// A MonoGame-based implementation of <see cref="IFontFactory"/> that loads,
    /// caches, and serves fonts using FontStashSharp.
    /// </summary>
    internal sealed class MonoGameFontFactory : IFontFactory, IDisposable
    {
        private readonly Dictionary<string, FontSystem> systems;
        private readonly Dictionary<(string family, int size), IFont> cache;

        public MonoGameFontFactory()
        {
            systems = new Dictionary<string, FontSystem>(StringComparer.OrdinalIgnoreCase);

            cache = new Dictionary<(string family, int size), IFont>();
        }

        public IFont GetFont(string path, int size = 128)
        {
            (string family, int size) key = (path, size);

            if (cache.TryGetValue(key, out IFont existing))
                return existing;

            if (!systems.TryGetValue(path, out var fontSystem))
            {
                fontSystem = RegisterFamily(path);
            }

            SpriteFontBase spriteFont = fontSystem.GetFont(size);

            MonoGameFont wrapped = new MonoGameFont(spriteFont, $"{path}-{size}");

            cache[key] = wrapped;
            return wrapped;
        }

        public bool TryGetFont(string path, out IFont? font, int size = 128)
        {
            var key = (path, size);

            if (cache.TryGetValue(key, out font))
                return true;

            if (!systems.TryGetValue(path, out var fontSystem))
            {
                try
                {
                    fontSystem = RegisterFamily(path);
                }
                catch (Exception)
                {
                    font = null;
                    return false;
                }
            }

            SpriteFontBase spriteFont = fontSystem.GetFont(size);

            font = new MonoGameFont(spriteFont, $"{path}-{size}");
            cache[key] = font;

            return true;
        }

        private FontSystem RegisterFamily(string ttfPath)
        {
            FontSystem fontSystem = new FontSystem(new FontSystemSettings { });

            fontSystem.AddFont(File.ReadAllBytes(ttfPath));

            systems[ttfPath] = fontSystem;
            return fontSystem;
        }

        public void Dispose()
        {
            foreach (FontSystem fontSystem in systems.Values)
            {
                fontSystem.Dispose();
            }

            systems.Clear();
            cache.Clear();
        }
    }
}