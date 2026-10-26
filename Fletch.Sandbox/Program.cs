using Fletch.Application;
using Fletch.Core.EngineConfig;
using Fletch.Sandbox;

FletchOptions options = new FletchOptions
{
    Width = 1920,
    Height = 1080,
    Title = "Fletch Sandbox",
    VSync = false,
    StartFullscreen = false,
    FixedUpdatesPerSecond = 60,
    MaxFixedUpdatesPerFrame = 5
};

FletchApplication.Run<TestGame>(options);