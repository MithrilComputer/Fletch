using Fletch.Input.Abstractions.Backends;
using Fletch.Input.MonoGame.Backend;
using Fletch.Platform.Abstractions.Contexts;
using Fletch.Platform.Abstractions.Lifecycle;
using Fletch.Platform.Abstractions.Paths;
using Fletch.Platform.Abstractions.Window;
using Fletch.Platform.MonoGame.Contexts;
using Fletch.Platform.MonoGame.Host;
using Fletch.Platform.MonoGame.Lifecycle;
using Fletch.Platform.MonoGame.Paths;
using Fletch.Platform.MonoGame.Window;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Rendering.MonoGame.Backend;
using Fletch.Runtime.Abstractions.Hosting;
using Fletch.Runtime.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;

namespace Fletch.Platform.MonoGame
{
    public static class MonoGameBootstrap
    {
        public static ServiceProvider CreateDefault(MonoGamePlatformOptions options)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            ServiceCollection services = new ServiceCollection();

            // Options + Paths
            services.AddSingleton(options);
            services.AddSingleton<IPathProvider>(sp => new MonoGamePathProvider(options.Title, options.MonoGameRootDirectory));

            // Game
            services.AddSingleton<FletchMonoGame>();
            services.AddSingleton<Game>(sp => sp.GetRequiredService<FletchMonoGame>());

            // Lifetime comes from the game instance
            services.AddSingleton<IApplicationLifetime, MonoGameAppLifetime>();

            // Platform pieces
            services.AddSingleton<IWindow, MonoGameWindow>();

            // Backends
            services.AddSingleton<IInputBackend, MonoGameInputBackend>();
            services.AddSingleton<IRenderingBackend, MonoGameRenderingBackend>();

            // Platform context container
            services.AddSingleton<IPlatformContext, MonoGamePlatformContext>();

            // Runtime (depends on IPlatformContext)
            services.AddSingleton<IRuntime, FletchRuntime>();
            services.AddSingleton<Func<IRuntime>>(sp => () => sp.GetRequiredService<IRuntime>());

            return services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });
        }
    }
}
