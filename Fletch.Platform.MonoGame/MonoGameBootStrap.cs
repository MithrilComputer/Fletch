using Fletch.Input.Abstractions.Backends;
using Fletch.Input.MonoGame.Backend;
using Fletch.Platform.Abstractions.Contexts;
using Fletch.Platform.Abstractions.Paths;
using Fletch.Platform.Abstractions.Window;
using Fletch.Platform.MonoGame.Contexts;
using Fletch.Platform.MonoGame.Host;
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
            if (options == null) throw new ArgumentNullException(nameof(options));

            ServiceCollection services = new ServiceCollection();

            FletchMonoGame game = new FletchMonoGame(options);

            MonoGamePathProvider pathProvider = new MonoGamePathProvider(options.Title);

            services.AddSingleton(game);
            services.AddSingleton<Game>(game);

            services.AddSingleton(pathProvider);
            services.AddSingleton<IPathProvider>(pathProvider);

            // Platform
            services.AddSingleton<IPlatformContext, MonoGamePlatformContext>();
            services.AddSingleton(game.ApplicationLifetime);
            services.AddSingleton<IWindow, MonoGameWindow>();

            // Runtime
            services.AddSingleton<IRuntime, FletchRuntime>();

            // Input 
            services.AddSingleton<IInputBackend, MonoGameInputBackend>();

            // Rendering
            services.AddSingleton<MonoGameRenderingBackend>();
            services.AddSingleton<IRenderingBackend>(sp =>
                sp.GetRequiredService<MonoGameRenderingBackend>());

            ServiceProvider provider = services.BuildServiceProvider(
                new ServiceProviderOptions
                {
                    ValidateScopes = false,
                    ValidateOnBuild = false
                });

            game.provider = provider;

            IRuntime runtime = provider.GetRequiredService<IRuntime>();

            game.Runtime = runtime;

            return provider;
        }
    }
}
