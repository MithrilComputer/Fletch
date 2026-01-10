namespace Fletch.Platform.Abstractions.Window
{
    /// <summary>
    /// Abstraction for an application window.
    /// Exposes common window state without platform-specific details.
    /// </summary>
    internal interface IWindow
    {
        /// <summary>
        /// Gets the current window title.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the window width in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the window height in pixels.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets whether the window is currently focused.
        /// </summary>
        bool IsFocused { get; }

        /// <summary>
        /// Gets whether the user can resize the window.
        /// </summary>
        bool IsResizable { get; }

        /// <summary>
        /// Fired when the window size changes.
        /// Parameters: width, height.
        /// </summary>
        event Action<int, int>? OnResize;

        /// <summary>
        /// Fired when the window focus changes.
        /// True when focused, false when unfocused.
        /// </summary>
        event Action<bool> OnFocusChange;
    }
}
