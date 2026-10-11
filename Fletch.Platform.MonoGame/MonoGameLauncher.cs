using Fletch.Core.EngineConfig;
using Fletch.Hosting.Abstractions.Game;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;

namespace Fletch.Platform.MonoGame
{
    internal class MonoGameLauncher : IPlatformLauncher
    {
        void IPlatformLauncher.Run<TApplication>(FletchOptions options)
        {
            using ServiceProvider provider = MonoGameBootstrap.CreateDefault(options);

            Game game = provider.GetRequiredService<Game>();

            game.Run();
        }
    }
}
