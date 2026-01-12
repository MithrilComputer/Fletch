using Fletch.Platform.MonoGame;
using Fletch.Platform.MonoGame.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;

MonoGamePlatformOptions options = new MonoGamePlatformOptions
{
    Width = 1920,
    Height = 1080,
    Title = "Game",
    VSync = false,
    TargetFramesPerSecond = 144,
    StartFullscreen = false,
    MonoGameRootDirectory = "TestContent"
};

using ServiceProvider provider = MonoGameBootstrap.CreateDefault(options);

Game game = provider.GetRequiredService<Game>();

game.Run();