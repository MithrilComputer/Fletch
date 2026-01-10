using Fletch.Input.Abstractions.Backends;
using Fletch.Platform.Abstractions.Contexts;
using Fletch.Platform.Abstractions.Lifecycle;
using Fletch.Platform.Abstractions.Paths;
using Fletch.Platform.Abstractions.Window;
using Fletch.Rendering.Abstractions.Backends;

namespace Fletch.Platform.MonoGame.Contexts
{
    /// <summary>
    /// MonoGame implementation of <see cref="IPlatformContext"/>.
    /// This is a simple container for platform-owned services.
    /// </summary>
    internal sealed class MonoGamePlatformContext : IPlatformContext
    {
        /// <summary>
        /// Gets the active application window.
        /// </summary>
        public IWindow Window { get; }

        /// <summary>
        /// Gets application lifetime controls (exit request, etc.).
        /// </summary>
        public IApplicationLifetime Lifetime { get; }

        /// <summary>
        /// Gets platform path locations (app data, cache, logs, content root).
        /// </summary>
        public IPathProvider PathProvider { get; }

        /// <summary>
        /// Gets the active rendering backend.
        /// </summary>
        public IRenderingBackend RenderingBackend { get; }

        /// <summary>
        /// Gets the active input backend.
        /// </summary>
        public IInputBackend InputBackend { get; }

        public MonoGamePlatformContext(
            IWindow window,
            IApplicationLifetime lifetime,
            IPathProvider paths,
            IRenderingBackend rendering,
            IInputBackend input)
        {
            Window = window ?? throw new ArgumentNullException(nameof(window));
            Lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
            PathProvider = paths ?? throw new ArgumentNullException(nameof(paths));
            RenderingBackend = rendering ?? throw new ArgumentNullException(nameof(rendering));
            InputBackend = input ?? throw new ArgumentNullException(nameof(input));
        }
    }
}
