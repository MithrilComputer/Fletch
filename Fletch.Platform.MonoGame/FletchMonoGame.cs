using Fletch.Core.Diagnostics;
using Fletch.Platform.Abstractions.Lifecycle;
using Fletch.Platform.Abstractions.Paths;
using Fletch.Platform.MonoGame.Diagnostics;
using Fletch.Platform.MonoGame.Host;
using Fletch.Platform.MonoGame.Lifecycle;
using Fletch.Platform.MonoGame.Loader;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Rendering.MonoGame.Backend;
using Fletch.Runtime.Abstractions.Hosting;
using Fletch.Runtime.Abstractions.Time;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using SDL2;

namespace Fletch.Platform.MonoGame
{
    /// <summary>
    /// MonoGame host game loop wrapper.
    /// </summary>
    internal class FletchMonoGame : Game
    {
        private readonly GraphicsDeviceManager graphics;

        private readonly MonoGamePlatformOptions options;

        private TimeSpan accumulatedTime = TimeSpan.Zero;

        private TimeSpan fixedTotalTime = TimeSpan.Zero;

        private float frameDelta = 0f;

        private float alpha = 0f;

        private ServiceProvider serviceProvider;

        private readonly IPathProvider pathProvider;

        private readonly Func<IRuntime> runtimeFactory;

        private IRuntime runtime;

        private readonly IApplicationLifetime applicationLifetime;

        private readonly IRenderingBackend renderingBackend;

        public FletchMonoGame(MonoGamePlatformOptions options, IPathProvider pathProvider, IRenderingBackend renderingBackend, Func<IRuntime> runtimeFactory)
        {
            this.options = options;

            this.runtimeFactory = runtimeFactory;

            this.renderingBackend = renderingBackend;

            this.pathProvider = pathProvider;

            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = options.Width;
            graphics.PreferredBackBufferHeight = options.Height;

            applicationLifetime = new MonoGameAppLifetime();

            Content.RootDirectory = pathProvider.BackendRoot;

            Window.Title = options.Title;

            graphics.SynchronizeWithVerticalRetrace = options.VSync;

            if (options.TargetFramesPerSecond >= 1)
            {
                IsFixedTimeStep = true;

                TargetElapsedTime = TimeSpan.FromSeconds(1.0 / options.TargetFramesPerSecond);
            } else
            {
                IsFixedTimeStep = false;
            }

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            Log.Current = new VSLogger();

            nint icon = IconLoader.GetIcon(Path.Combine(AppContext.BaseDirectory, "Icon.png"));

            SDL.SDL_SetWindowIcon(Window.Handle, icon);
            SDL.SDL_FreeSurface(icon);
        }

        protected override void LoadContent()
        {
            if (renderingBackend is not MonoGameRenderingBackend renderer)
                throw new InvalidCastException();

            renderer.Initialize(this);

            runtime = runtimeFactory();
            runtime.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            if (applicationLifetime?.IsExitRequested == true)
            {
                Exit();
            }

            frameDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (frameDelta > 0.25f) frameDelta = 0.25f;
            if (frameDelta < 0f) frameDelta = 0f;

            accumulatedTime += TimeSpan.FromSeconds(frameDelta);

            int fixedHz = Math.Max(1, options.FixedUpdatesPerSecond);
            double fixedDelta = 1.0 / fixedHz;
            TimeSpan fixedStep = TimeSpan.FromSeconds(fixedDelta);

            for (int steps = 0; accumulatedTime >= fixedStep && steps < options.MaxFixedUpdatesPerFrame; steps++)
            {
                fixedTotalTime += fixedStep;

                runtime?.FixedUpdate(new FixedTimeStep((float)fixedDelta, fixedTotalTime));

                accumulatedTime -= fixedStep;
            }

            if (accumulatedTime >= fixedStep)
                accumulatedTime = fixedStep;

            alpha = (float)(accumulatedTime.TotalSeconds / fixedStep.TotalSeconds);
            alpha = Math.Clamp(alpha, 0f, 1f);

            runtime?.Update(new FrameTime(frameDelta, fixedTotalTime));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            runtime?.Render(new FrameTime(frameDelta, fixedTotalTime, alpha));

            base.Draw(gameTime);
        }
    }
}
