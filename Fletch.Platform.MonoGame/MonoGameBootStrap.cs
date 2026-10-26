using Fletch.Assets.Audio.Providers;
using Fletch.Audio.Abstractions.Assets;
using Fletch.Audio.Abstractions.Backend;
using Fletch.Audio.Silk.NET.OpenAL.Backend;
using Fletch.Core.Diagnostics;
using Fletch.Core.EngineConfig;
using Fletch.Core.Platform;
using Fletch.Engine.Abstractions.Factories;
using Fletch.Engine.Factories;
using Fletch.Engine.Systems.Scenes;
using Fletch.Hosting.Abstractions.Contexts;
using Fletch.Hosting.Abstractions.Game;
using Fletch.Hosting.API;
using Fletch.Hosting.Contexts;
using Fletch.Input.Abstractions.Backends;
using Fletch.Input.API;
using Fletch.Input.MonoGame.Backend;
using Fletch.Physics.Abstractions.Backends;
using Fletch.Physics.Box2D.Backend;
using Fletch.Platform.Abstractions.Contexts;
using Fletch.Platform.Abstractions.Lifecycle;
using Fletch.Platform.Abstractions.Paths;
using Fletch.Platform.Abstractions.Window;
using Fletch.Platform.MonoGame.Contexts;
using Fletch.Platform.MonoGame.Diagnostics;
using Fletch.Platform.MonoGame.Lifecycle;
using Fletch.Platform.MonoGame.Paths;
using Fletch.Platform.MonoGame.Window;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Rendering.Abstractions.Managers;
using Fletch.Rendering.API;
using Fletch.Rendering.Managers;
using Fletch.Rendering.MonoGame.Backend;
using Fletch.Rendering.Systems;
using Fletch.Runtime.Abstractions.Hosting;
using Fletch.Runtime.Factories;
using Fletch.Runtime.Hosting;
using Fletch.Runtime.Systems;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;

namespace Fletch.Platform.MonoGame
{
    public static class MonoGameBootstrap
    {
        public static ServiceProvider CreateDefault<TGame>(FletchOptions options, Action<IServiceCollection>? configureServices = null) where TGame : class, IFletchGame
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            ServiceCollection services = new ServiceCollection();

            // Logging
            services.AddSingleton<IFletchLogger, VSLogger>();
            services.AddSingleton(typeof(IFletchContextLogger<>), typeof(VSContextLogger<>));

            // Options + Paths
            services.AddSingleton(options);
            services.AddSingleton<IPathProvider>(sp => new MonoGamePathProvider(options.Title));

            // Game
            services.AddSingleton<FletchMonoGame>();
            services.AddSingleton<Game>(sp => sp.GetRequiredService<FletchMonoGame>());

            // Lifetime comes from the game instance
            services.AddSingleton<IApplicationLifetime, MonoGameAppLifetime>();

            // Platform pieces
            services.AddSingleton<IWindow, MonoGameWindow>();

            // Rendering
            services.AddSingleton<IRenderingBackend, MonoGameRenderingBackend>();
            services.AddSingleton<ICameraManager, CameraManager>();
            services.AddSingleton<IRenderSurface, MonoGameRenderSurface>();
            services.AddTransient<WorldRenderingSystem>();

            // Input
            services.AddSingleton<IInputBackend, MonoGameInputBackend>();

            // Audio
            services.AddSingleton<IAudioBackend, OpenALAudioBackend>();

            // Physics
            services.AddSingleton<IPhysicsBackend, Box2DPhysicsBackend>();

            // Asset Providers
            services.AddSingleton<IAudioAssetProvider, AudioAssetProvider>();

            // Platform context container
            services.AddSingleton<IPlatformContext, MonoGamePlatformContext>();

            // Runtime
            services.AddSingleton<IRuntime, FletchRuntime>();
            services.AddSingleton<Func<IRuntime>>(sp => () => sp.GetRequiredService<IRuntime>());

            services.AddSingleton<PhysicsInterpolator>();

            // Engine Factories
            services.AddSingleton<IGameObjectFactory, GameObjectFactory>();
            services.AddSingleton<ISceneFactory, SceneFactory>();
            services.AddSingleton<ISubSystemFactory, SubSystemFactory>();
            services.AddSingleton<IComponentFactory, ComponentFactory>();

            // Public API
            services.AddSingleton<IFletchGame, TGame>();

            services.AddSingleton(sp => new RenderingResources(sp.GetRequiredService<IRenderingBackend>()));
            services.AddSingleton(sp => new InputService(sp.GetRequiredService<IInputBackend>()));
            services.AddSingleton(sp => new SceneManager(sp.GetRequiredService<ISceneFactory>()));
            services.AddSingleton(sp => new ApplicationLifetime(sp.GetRequiredService<IApplicationLifetime>()));

            services.AddSingleton<IGameContext, FletchGameContext>();

            configureServices?.Invoke(services);

            return services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });
        }
    }
}
