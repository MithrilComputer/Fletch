using Fletch.Core.Platform;
using Fletch.Platform.Abstractions.Window;
using Microsoft.Xna.Framework.Graphics;

namespace Fletch.Platform.MonoGame.Window
{
    internal class MonoGameRenderSurface : IRenderSurface
    {
        private IWindow window;
        private GraphicsDevice graphicsDevice;

        public MonoGameRenderSurface(IWindow window, GraphicsDevice graphics)
        {
            this.window = window;
            this.graphicsDevice = graphics;
        }

        public int Width => window.Width;

        public int Height => window.Height;

        public bool IsValid =>
            graphicsDevice != null &&
            !graphicsDevice.IsDisposed &&
            (window.Width > 0 && window.Height > 0);
    }
}
