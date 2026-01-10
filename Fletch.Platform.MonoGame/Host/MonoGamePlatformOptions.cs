namespace Fletch.Platform.MonoGame.Host
{
    public sealed class MonoGamePlatformOptions
    {
        public int Width { get; init; } = 1280;

        public int Height { get; init; } = 720;

        public string Title { get; init; } = "Fletch";

        public bool VSync { get; init; } = true;

        public int TargetFramesPerSecond { get; init; } = 60;

        public int FixedUpdatesPerSecond { get; init; } = 60;

        public int MaxFixedUpdatesPerFrame { get; init; } = 5;

        public bool StartFullscreen { get; init; } = false;

        public string MonoGameRootDirectory { get; init; } = "Content";
    }
}
