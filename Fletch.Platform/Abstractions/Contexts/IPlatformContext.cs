using Fletch.Audio.Abstractions.Backend;
using Fletch.Input.Abstractions.Backends;
using Fletch.Platform.Abstractions.Lifecycle;
using Fletch.Platform.Abstractions.Paths;
using Fletch.Platform.Abstractions.Window;
using Fletch.Rendering.Abstractions.Backends;

namespace Fletch.Platform.Abstractions.Contexts
{
    /// <summary>
    /// Provides runtime access to platform-owned services (window, lifecycle, paths, and platform backends).
    /// </summary>
    internal interface IPlatformContext
    {
        /// <summary>
        /// Gets the active application window.
        /// </summary>
        IWindow Window { get; }

        /// <summary>
        /// Gets application lifetime controls (exit request, etc.).
        /// </summary>
        IApplicationLifetime Lifetime { get; }

        /// <summary>
        /// Gets platform path locations (app data, cache, logs, content root).
        /// </summary>
        IPathProvider PathProvider { get; }

        /// <summary>
        /// Gets the active rendering backend.
        /// </summary>
        IRenderingBackend RenderingBackend { get; }

        /// <summary>
        /// Gets the active input backend.
        /// </summary>
        IInputBackend InputBackend { get; }

        /// <summary>
        /// Gets the audio backend in use.
        /// </summary>
        //IAudioBackend AudioBackend { get; } TODO
    }
}
