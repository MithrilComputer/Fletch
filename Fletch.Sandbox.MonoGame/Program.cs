using Fletch.Platform.MonoGame;
using Fletch.Platform.MonoGame.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;

MonoGamePlatformOptions options = new MonoGamePlatformOptions
{
    Width = 1280,
    Height = 720,
    Title = "Fletch Sandbox",
    VSync = false,
    StartFullscreen = false,
    FixedUpdatesPerSecond = 60,
    MaxFixedUpdatesPerFrame = 5
};

using ServiceProvider provider = MonoGameBootstrap.CreateDefault(options);

Game game = provider.GetRequiredService<Game>();

game.Run();