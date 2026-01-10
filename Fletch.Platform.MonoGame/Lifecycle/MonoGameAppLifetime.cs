using Fletch.Platform.Abstractions.Lifecycle;

namespace Fletch.Platform.MonoGame.Lifecycle
{
    internal class MonoGameAppLifetime : IApplicationLifetime
    {
        private bool exitRequested = false;

        /// <summary>
        /// Gets whether an exit has been requested.
        /// </summary>
        public bool IsExitRequested => exitRequested;

        /// <summary>
        /// Occurs when the application is beginning to exit.
        /// </summary>
        public event Action? Exiting;

        /// <summary>
        /// Requests the application to exit.
        /// The platform host should honor this request at a safe time.
        /// </summary>
        public void RequestExit()
        {
            Exiting?.Invoke();

            exitRequested = true;
        }
    }
}
