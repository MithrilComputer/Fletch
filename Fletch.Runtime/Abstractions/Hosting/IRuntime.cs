using Fletch.Platform.Abstractions.Contexts;
using Fletch.Runtime.Abstractions.Time;

namespace Fletch.Runtime.Abstractions.Hosting
{
    /// <summary>
    /// Defines the engine runtime lifecycle, driven by a platform host.
    /// </summary>
    internal interface IRuntime
    {
        /// <summary>
        /// Gets the <see cref="IPlatformContext"/> this runtime was created with.
        /// </summary>
        IPlatformContext Platform { get; }

        /// <summary>
        /// True once <see cref="Initialize"/> has completed successfully.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Platforms one-time initialization for the runtime.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Advances simulation and game logic by one frame.
        /// </summary>
        void Update(FrameTime time);

        /// <summary>
        /// Advances the Fixed simulation by one tick.
        /// </summary>
        void FixedUpdate(FixedTimeStep time);

        /// <summary>
        /// Renders the current frame.
        /// </summary>
        void Render(FrameTime time);

        void Pause(); //TODO

        void Start(); //TODO
    }
}
