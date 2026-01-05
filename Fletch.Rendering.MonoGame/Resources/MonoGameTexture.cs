using Fletch.Rendering.Abstractions.Resources;
using Microsoft.Xna.Framework.Graphics;

namespace Fletch.Rendering.MonoGame.Resources
{
    /// <summary>
    /// The MonoGame implementation of a texture resource.
    /// </summary>
    internal sealed class MonoGameTexture : ITexture, IDisposable
    {
        private readonly Texture2D texture;

        /// <summary>
        /// The underlying MonoGame Texture2D instance.
        /// </summary>
        public Texture2D Texture => texture;

        /// <summary>
        /// The width of the texture in pixels.
        /// </summary>
        public int Width => texture.Width;

        /// <summary>
        /// The height of the texture in pixels.
        /// </summary>
        public int Height => texture.Height;

        public MonoGameTexture(Texture2D texture)
        {
            this.texture = texture;
        }

        public void Dispose()
        {
            texture?.Dispose();
        }
    }
}
