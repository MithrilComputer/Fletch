using Fletch.Core.Time;
using Fletch.Platform.Abstractions.Lifecycle;
using Fletch.Platform.Abstractions.Paths;
using Fletch.Platform.MonoGame.Host;
using Fletch.Platform.MonoGame.Lifecycle;
using Fletch.Platform.MonoGame.Loader;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Rendering.MonoGame.Backend;
using Fletch.Runtime.Abstractions.Hosting;
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


        private int fixedHz;

        private double fixedDelta;

        private TimeSpan fixedStep;

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

            Window.Title = options.Title;

            graphics.SynchronizeWithVerticalRetrace = options.VSync;

            fixedHz = Math.Max(1, options.FixedUpdatesPerSecond);

            fixedDelta = 1.0 / fixedHz;

            fixedStep = TimeSpan.FromSeconds(fixedDelta);

            Window.AllowUserResizing = true;

            IsFixedTimeStep = false;

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

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
                return;
            }

            frameDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (frameDelta > 0.25f) frameDelta = 0.25f;
            if (frameDelta < 0f) frameDelta = 0f;

            accumulatedTime += TimeSpan.FromSeconds(frameDelta);

            for (int steps = 0; accumulatedTime >= fixedStep && steps < options.MaxFixedUpdatesPerFrame; steps++)
            {
                fixedTotalTime += fixedStep;

                runtime?.FixedUpdate(new FixedTimeStep((float)fixedDelta, fixedTotalTime));

                accumulatedTime -= fixedStep;
            }

            if (accumulatedTime >= fixedStep)
            {
                accumulatedTime = TimeSpan.FromTicks(
                    accumulatedTime.Ticks % fixedStep.Ticks);
            }

            alpha = (float)(accumulatedTime.TotalSeconds / fixedStep.TotalSeconds);
            alpha = Math.Clamp(alpha, 0f, 1f);

            runtime?.Update(new FrameTime(frameDelta, fixedTotalTime, alpha)); // FixedTotal time should be replaced with alpha or somthin

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            runtime?.Render(new FrameTime(frameDelta, fixedTotalTime, alpha));

            base.Draw(gameTime);
        }
    }
}
