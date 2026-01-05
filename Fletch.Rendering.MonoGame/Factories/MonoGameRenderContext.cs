using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.MonoGame.Resources;
using Microsoft.Xna.Framework.Graphics;

namespace Fletch.Rendering.MonoGame.Factories
{
    internal sealed class MonoGameRenderContext
    {
        private readonly GraphicsDevice graphicsDevice;

        public MonoGameRenderContext(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public void SetRenderTarget(IRenderingTarget? renderTarget)
        {
            if (renderTarget is MonoGameRenderTarget monoGameRenderTarget)
            {
                graphicsDevice.SetRenderTarget(monoGameRenderTarget.RenderTarget);
            }
            else
            {
                graphicsDevice.SetRenderTarget(null);
            }
        }
    }
}
