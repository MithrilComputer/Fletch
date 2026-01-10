using Fletch.Platform.Abstractions.Window;
using Microsoft.Xna.Framework;

namespace Fletch.Platform.MonoGame.Window
{
    internal sealed class MonoGameWindow : IWindow, IDisposable
    {
        private readonly Game game;

        private readonly GameWindow window;

        /// <summary>
        /// Fired when the window size changes.
        /// Parameters: width, height.
        /// </summary>
        public event Action<int, int>? OnResize;

        /// <summary>
        /// Fired when the window focus changes.
        /// True when focused, false when unfocused.
        /// </summary>
        public event Action<bool>? OnFocusChange;

        /// <summary>
        /// Gets the current window title.
        /// </summary>
        public string Title
        {
            get => window.Title;
            set => window.Title = value ?? string.Empty;
        }

        /// <summary>
        /// Gets the window width in pixels.
        /// </summary>
        public int Width => window.ClientBounds.Width;

        /// <summary>
        /// Gets the window height in pixels.
        /// </summary>
        public int Height => window.ClientBounds.Height;

        /// <summary>
        /// Gets whether the window is currently focused.
        /// </summary>
        public bool IsFocused => game.IsActive;

        /// <summary>
        /// Gets whether the user can resize the window.
        /// </summary>
        public bool IsResizable
        {
            get => window.AllowUserResizing;
            set => window.AllowUserResizing = value;
        }

        public MonoGameWindow(Game game)
        {
            this.game = game;

            window = game.Window;

            window.ClientSizeChanged += OnClientSizeChanged;
            game.Activated += OnActivated;
            game.Deactivated += OnDeactivated;
        }

        private void OnClientSizeChanged(object? sender, EventArgs e)
        {
            var bounds = game.Window.ClientBounds;
            OnResize?.Invoke(bounds.Width, bounds.Height);
        }

        private void OnActivated(object? sender, EventArgs e)
        {
            OnFocusChange?.Invoke(true);
        }

        private void OnDeactivated(object? sender, EventArgs e)
        {
            OnFocusChange?.Invoke(false);
        }

        public void Dispose()
        {
            window.ClientSizeChanged -= OnClientSizeChanged;
            game.Activated -= OnActivated;
            game.Deactivated -= OnDeactivated;
        }
    }
}
