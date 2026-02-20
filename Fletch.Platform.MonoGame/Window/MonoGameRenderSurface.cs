using Fletch.Core.Platform;
using Fletch.Platform.Abstractions.Window;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fletch.Platform.MonoGame.Window
{
    internal class MonoGameRenderSurface : IRenderSurface
    {
        private IWindow window;
        private GraphicsDevice graphicsDevice;

        public MonoGameRenderSurface(IWindow window, Game game)
        {
            this.window = window;
            this.graphicsDevice = game.GraphicsDevice;
        }

        public int Width => window.Width;

        public int Height => window.Height;

        public bool IsValid =>
            graphicsDevice != null &&
            !graphicsDevice.IsDisposed &&
            (window.Width > 0 && window.Height > 0);
    }
}
