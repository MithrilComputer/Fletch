using Fletch.Platform.Abstractions.Lifecycle;
using Fletch.Platform.MonoGame.Host;
using Fletch.Platform.MonoGame.Lifecycle;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Rendering.MonoGame.Backend;
using Fletch.Runtime.Abstractions.Hosting;
using Fletch.Runtime.Abstractions.Time;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;

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

        public ServiceProvider provider;

        public IRuntime? Runtime { get; set; }

        public IApplicationLifetime ApplicationLifetime { get; }

        public FletchMonoGame(MonoGamePlatformOptions options)
        {
            this.options = options;

            ApplicationLifetime = new MonoGameAppLifetime();

            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = options.MonoGameRootDirectory;

            Window.Title = options.Title;

            graphics.PreferredBackBufferWidth = options.Width;
            graphics.PreferredBackBufferHeight = options.Height;

            graphics.SynchronizeWithVerticalRetrace = options.VSync;

            IsFixedTimeStep = false;

            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            provider.GetRequiredService<MonoGameRenderingBackend>().Initialize(this);
            Runtime?.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            if (ApplicationLifetime?.IsExitRequested == true)
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

                Runtime?.FixedUpdate(new FixedTimeStep((float)fixedDelta, fixedTotalTime));

                accumulatedTime -= fixedStep;
            }

            if (accumulatedTime >= fixedStep)
                accumulatedTime = fixedStep;

            alpha = (float)(accumulatedTime.TotalSeconds / fixedStep.TotalSeconds);
            alpha = Math.Clamp(alpha, 0f, 1f);

            Runtime?.Update(new FrameTime(frameDelta, fixedTotalTime));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Runtime?.Render(new FrameTime(frameDelta, fixedTotalTime, alpha));

            base.Draw(gameTime);
        }
    }
}
