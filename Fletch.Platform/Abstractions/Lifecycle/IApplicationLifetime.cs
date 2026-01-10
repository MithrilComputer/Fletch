namespace Fletch.Platform.Abstractions.Lifecycle
{
    /// <summary>
    /// Controls application lifetime in a platform-agnostic way.
    /// </summary>
    internal interface IApplicationLifetime
    {
        /// <summary>
        /// Gets whether an exit has been requested.
        /// </summary>
        bool IsExitRequested { get; }

        /// <summary>
        /// Requests the application to exit.
        /// The platform host should honor this request at a safe time.
        /// </summary>
        void RequestExit();

        /// <summary>
        /// Occurs when the application is beginning to exit.
        /// </summary>
        event Action? Exiting;
    }
}
