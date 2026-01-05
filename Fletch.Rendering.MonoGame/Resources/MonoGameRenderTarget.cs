using Fletch.Rendering.Abstractions.Resources;
using Microsoft.Xna.Framework.Graphics;

namespace Fletch.Rendering.MonoGame.Resources
{
    /// <summary>
    /// The MonoGame implementation of a rendering target resource.
    /// </summary>
    internal sealed class MonoGameRenderTarget : IRenderingTarget, IDisposable
    {
        /// <summary>
        /// The underlying MonoGame RenderTarget2D instance.
        /// </summary>
        public RenderTarget2D RenderTarget { get; }

        /// <summary>
        /// The width of the render target in pixels.
        /// </summary>
        public int Width => RenderTarget.Width;

        /// <summary>
        /// The height of the render target in pixels.
        /// </summary>
        public int Height => RenderTarget.Height;

        public MonoGameRenderTarget(RenderTarget2D renderTarget)
        {
            RenderTarget = renderTarget ?? throw new ArgumentNullException(nameof(renderTarget));
        }

        public void Dispose()
        {
            RenderTarget?.Dispose();
        }
    }
}
