using Fletch.Core.EngineConfig;
using Fletch.Core.Time;
using Fletch.Engine.Abstractions.Factories;
using Fletch.Input.Abstractions.Backends;
using Fletch.Platform.Abstractions.Contexts;
using Fletch.Platform.Abstractions.Lifecycle;
using Fletch.Platform.Abstractions.Paths;
using Fletch.Platform.Abstractions.Window;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Runtime.Abstractions.Hosting;

namespace Fletch.Runtime.Hosting
{
    internal class FletchRuntime : IRuntime
    {
        private readonly IWindow window;

        private readonly IApplicationLifetime applicationLifeTime;

        private readonly IPathProvider pathProvider;

        private readonly IRenderingBackend renderingBackend;

        private readonly IInputBackend inputBackend;

        private readonly ISceneFactory sceneFactory;

        private readonly ISubSystemFactory subSystemFactory;

        public IPlatformContext Platform => throw new NotImplementedException();

        public bool IsInitialized { get; private set; }

        public FletchRuntime(IPlatformContext platformContext, ISceneFactory sceneFactory, ISubSystemFactory subSystemFactory)
        {
            window = platformContext.Window;
            applicationLifeTime = platformContext.Lifetime;
            pathProvider = platformContext.PathProvider;
            renderingBackend = platformContext.RenderingBackend;
            inputBackend = platformContext.InputBackend;
            

            this.sceneFactory = sceneFactory;
            this.subSystemFactory = subSystemFactory;
        }

        public void Initialize()
        {
            IsInitialized = true;

            //Initialize the Active Scene
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

            //Start the Active Scene
        }

        public void Update(FrameTime time)
        {
            if (!IsInitialized)
                return;

            inputBackend.UpdateBackend();

            //Update the Active Scene
        }

        public void FixedUpdate(FixedTimeStep time)
        {
            if (!IsInitialized)
                return;

            //Fixed Update the Active Scene
        }

        public void Render(FrameTime time)
        {
            if (!IsInitialized)
                return;

            renderingBackend.BeginFrame(EngineConfig.ClearColor);

            //Render Active Scene

            renderingBackend.EndFrame();
        }
    }
}
