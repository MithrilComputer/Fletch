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

        /// <summary>
        /// Registers a font family from a TrueType Font (.ttf) file.
        /// </summary>
        /// <param name="name">Logical font family name used for lookup.</param>
        /// <param name="ttfPath">File system path to the .ttf font file.</param>
        /// <remarks>
        /// If a font family with the same name already exists, it will be replaced.
        /// </remarks>
        public void RegisterFamily(string name, string ttfPath)
        {
            FontSystem fontSystem = new FontSystem(new FontSystemSettings { });

            fontSystem.AddFont(File.ReadAllBytes(ttfPath));

            systems[name] = fontSystem;
        }

        /// <summary>
        /// Gets a font from the specified family and size.
        /// Creates and caches the font if it has not already been created.
        /// </summary>
        /// <param name="family">Registered font family name.</param>
        /// <param name="size">Point size of the requested font.</param>
        /// <returns>An <see cref="IFont"/> matching the specified family and size.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when the requested font family has not been registered.
        /// </exception>
        public IFont GetFont(string family, int size)
        {
            var key = (family, size);

            if (cache.TryGetValue(key, out var existing))
                return existing;

            if (!systems.TryGetValue(family, out var fontSystem))
                throw new KeyNotFoundException($"Font family '{family}' is not registered.");

            SpriteFontBase spriteFont = fontSystem.GetFont(size);

            MonoGameFont wrapped = new MonoGameFont(spriteFont, $"{family}-{size}");

            cache[key] = wrapped;
            return wrapped;
        }

        /// <summary>
        /// Attempts to get a font from the specified family and size without throwing.
        /// </summary>
        /// <param name="family">Registered font family name.</param>
        /// <param name="size">Point size of the requested font.</param>
        /// <param name="font">
        /// When this method returns, contains the font if it was found or created;
        /// otherwise <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the font could be returned; otherwise <c>false</c>.
        /// </returns>
        public bool TryGetFont(string family, int size, out IFont? font)
        {
            var key = (family, size);

            if (cache.TryGetValue(key, out font))
                return true;

            if (!systems.TryGetValue(family, out var fontSystem))
            {
                font = null;
                return false;
            }

            SpriteFontBase spriteFont = fontSystem.GetFont(size);

            font = new MonoGameFont(spriteFont, $"{family}-{size}");
            cache[key] = font;

            return true;
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