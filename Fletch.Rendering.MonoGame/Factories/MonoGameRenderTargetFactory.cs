using Fletch.Rendering.Abstractions.Factories;
using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.MonoGame.Resources;
using Microsoft.Xna.Framework.Graphics;

namespace Fletch.Rendering.MonoGame.Factories
{
    /// <summary>
    /// The main class for making render targets for MonoGame.
    /// </summary>
    internal sealed class MonoGameRenderTargetFactory : IRenderingTargetFactory
    {
        private readonly GraphicsDevice graphicsDevice;

        public MonoGameRenderTargetFactory(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        /// <summary>
        /// Creates a new render target Texture.
        /// </summary>
        public IRenderingTarget Create(int width, int height)
        {
            var renderTarget = new RenderTarget2D(
                graphicsDevice,
                width,
                height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None);
            return new MonoGameRenderTarget(renderTarget);
        }
    }
}
