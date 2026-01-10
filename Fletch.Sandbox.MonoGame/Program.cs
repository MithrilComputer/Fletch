using Fletch.Platform.MonoGame;
using Fletch.Platform.MonoGame.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;

MonoGamePlatformOptions options = new MonoGamePlatformOptions();

using ServiceProvider provider = MonoGameBootstrap.CreateDefault(options);

Game game = provider.GetRequiredService<Game>();

game.Run();