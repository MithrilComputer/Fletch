namespace Fletch.Platform.Abstractions.Paths
{
    /// <summary>
    /// Provides platform-specific filesystem paths used by the engine.
    /// </summary>
    public interface IPathProvider
    {
        /// <summary>
        /// Gets the root directory for application data (saves, settings).
        /// </summary>
        string AppDataPath { get; }

        /// <summary>
        /// Gets the directory used for cached data (temporary, regenerable).
        /// </summary>
        string CachePath { get; }

        /// <summary>
        /// Gets the directory used for log files.
        /// </summary>
        string LogsPath { get; }

        /// <summary>
        /// Gets the root directory for game assets.
        /// </summary>
        public string EngineContentRoot { get; }

        /// <summary>
        /// Gets the root directory for the optional backend files.
        /// </summary>
        public string BackendContentRoot { get; }
    }
}
