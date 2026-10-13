using Fletch.Core.EngineConfig;
using Fletch.Hosting.Abstractions.Game;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;

namespace Fletch.Platform.MonoGame
{
    internal class MonoGameLauncher : IPlatformLauncher
    {
        void IPlatformLauncher.Run<TGame>(FletchOptions options, Action<IServiceCollection>? configureServices = null)
        {
            using ServiceProvider provider = MonoGameBootstrap.CreateDefault<TGame>(options, configureServices);

            Game game = provider.GetRequiredService<Game>();

            game.Run();
        }
    }
}
