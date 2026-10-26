using Fletch.Core.EngineConfig;
using Fletch.Core.Time;
using Fletch.Engine.Systems.Scenes;
using Fletch.Hosting.Abstractions.Contexts;
using Fletch.Hosting.Abstractions.Game;
using Fletch.Input.Abstractions.Backends;
using Fletch.Platform.Abstractions.Contexts;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Runtime.Abstractions.Hosting;

namespace Fletch.Runtime.Hosting
{
    internal class FletchRuntime : IRuntime
    {
        private readonly SceneManager sceneManager;

        private readonly IRenderingBackend renderingBackend;

        private readonly IInputBackend inputBackend;

        private readonly IFletchGame game;

        private readonly IGameContext gameContext;

        public IPlatformContext Platform => throw new NotImplementedException();

        public bool IsInitialized { get; private set; } = false;

        public FletchRuntime(IPlatformContext platformContext, SceneManager sceneManager, IFletchGame game, IGameContext gameContext)
        {
            renderingBackend = platformContext.RenderingBackend;
            inputBackend = platformContext.InputBackend;

            this.gameContext = gameContext;
            this.sceneManager = sceneManager;
            this.game = game;
        }

        public void Initialize()
        {
            if (IsInitialized)
                return;

            game.Initialize(gameContext);

            IsInitialized = true;
        }

        public void Pause()
        {
            if (!IsInitialized)
                return;

            //Pause the Active Scene
        }

        public void Start()
        {
            if (!IsInitialized)
                return;

            game.Start();
        }

        public void Update(FrameTime time)
        {
            if (!IsInitialized)
                return;

            inputBackend.UpdateBackend();

            sceneManager.ActiveScene?.Update(time);
        }

        public void FixedUpdate(FixedTimeStep time)
        {
            if (!IsInitialized)
                return;

            sceneManager.ActiveScene?.FixedUpdate(time);
        }

        public void Render(FrameTime time)
        {
            if (!IsInitialized)
                return;

            renderingBackend.BeginFrame(EngineConfig.ClearColor);

            sceneManager.ActiveScene?.Render(time);

            renderingBackend.EndFrame();
        }
    }
}
