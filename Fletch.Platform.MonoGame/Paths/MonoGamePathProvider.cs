using Fletch.Platform.Abstractions.Paths;

namespace Fletch.Platform.MonoGame.Paths
{
    internal class MonoGamePathProvider : IPathProvider
    {
        /// <summary>
        /// Gets the root directory for application data (saves, settings).
        /// </summary>
        public string AppDataPath { get; }

        /// <summary>
        /// Gets the directory used for cached data (temporary, regenerable).
        /// </summary>
        public string CachePath { get; }

        /// <summary>
        /// Gets the directory used for log files.
        /// </summary>
        public string LogsPath { get; }

        /// <summary>
        /// Gets the root directory for game assets.
        /// </summary>
        public string EngineContentRoot { get; }

        /// <summary>
        /// Gets the root directory for the optional backend files.
        /// </summary>
        public string BackendContentRoot { get; }

        public MonoGamePathProvider(string applicationName, string? backendDir = null)
        {
            if (string.IsNullOrWhiteSpace(applicationName))
                throw new ArgumentException("Application name must be provided.", nameof(applicationName));

            string baseAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            AppDataPath = Path.Combine(baseAppData, applicationName);
            CachePath = Path.Combine(AppDataPath, "Cache");
            LogsPath = Path.Combine(AppDataPath, "Logs");


            EngineContentRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "Assets"));

            BackendContentRoot = Path.GetFullPath(
                string.IsNullOrWhiteSpace(backendDir)
                    ? Path.Combine(AppContext.BaseDirectory, "Content")
                    : backendDir);

            EnsureDirectory(AppDataPath);
            EnsureDirectory(CachePath);
            EnsureDirectory(LogsPath);
            EnsureDirectory(EngineContentRoot);
        }

        private static void EnsureDirectory(string path)
        {
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
