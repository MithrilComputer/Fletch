using Fletch.Audio.Abstractions.Assets;
using Fletch.Core.Diagnostics;
using Fletch.Core.Platform;
using Fletch.Engine.Abstractions.Factories;
using Fletch.Engine.Factories;
using Fletch.Input.Abstractions.Backends;
using Fletch.Input.MonoGame.Backend;
using Fletch.Platform.Abstractions.Contexts;
using Fletch.Platform.Abstractions.Lifecycle;
using Fletch.Platform.Abstractions.Paths;
using Fletch.Platform.Abstractions.Window;
using Fletch.Platform.MonoGame.Contexts;
using Fletch.Platform.MonoGame.Diagnostics;
using Fletch.Platform.MonoGame.Host;
using Fletch.Platform.MonoGame.Lifecycle;
using Fletch.Platform.MonoGame.Paths;
using Fletch.Platform.MonoGame.Window;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Rendering.Abstractions.Managers;
using Fletch.Rendering.Managers;
using Fletch.Rendering.MonoGame.Backend;
using Fletch.Rendering.Systems;
using Fletch.Runtime.Abstractions.Hosting;
using Fletch.Runtime.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Fletch.Assets.Audio.Providers;
using Fletch.Audio.Abstractions.Backend;
using Fletch.Audio.Silk.NET.OpenAL.Backend;

namespace Fletch.Platform.MonoGame
{
    public static class MonoGameBootstrap
    {
        public static ServiceProvider CreateDefault(MonoGamePlatformOptions options)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            ServiceCollection services = new ServiceCollection();

            // Logging
            services.AddSingleton<IFletchLogger, VSLogger>();
            services.AddSingleton(typeof(IFletchContextLogger<>), typeof(VSContextLogger<>));

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

            //Rendering
            services.AddSingleton<ICameraManager, CameraManager>();
            services.AddSingleton<IRenderSurface, MonoGameRenderSurface>();
            services.AddTransient<WorldRenderingSystem>();

            // Audio
            services.AddSingleton<IAudioBackend, OpenALAudioBackend>();

            // Asset Providers
            services.AddSingleton<IAudioAssetProvider, AudioAssetProvider>();

            // Platform context container
            services.AddSingleton<IPlatformContext, MonoGamePlatformContext>();

            // Runtime
            services.AddSingleton<IRuntime, FletchRuntime>();
            services.AddSingleton<Func<IRuntime>>(sp => () => sp.GetRequiredService<IRuntime>());

            // Engine Factories
            services.AddSingleton<IGameObjectFactory, GameObjectFactory>();
            services.AddSingleton<ISceneFactory, SceneFactory>();
            services.AddSingleton<ISubSystemFactory, SubSystemFactory>();
            services.AddSingleton<IComponentFactory, ComponentFactory>();

            return services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });
        }
    }
}
