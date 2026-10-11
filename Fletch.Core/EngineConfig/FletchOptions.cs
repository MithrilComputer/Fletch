namespace Fletch.Core.EngineConfig
{
    /// <summary>
    /// Encapsulates configuration options for the Fletch application.
    /// </summary>
    public class FletchOptions
    {
        public string Title { get; set; } = "Fletch Application";

        public int Width { get; set; } = 1280;
        public int Height { get; set; } = 720;

        public bool VSync { get; set; } = true;
        public bool StartFullscreen { get; set; } = false;

        public int FixedUpdatesPerSecond { get; set; } = 60;
        public int MaxFixedUpdatesPerFrame { get; set; } = 5;
    }
}
