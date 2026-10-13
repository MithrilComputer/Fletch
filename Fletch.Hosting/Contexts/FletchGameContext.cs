using Fletch.Engine.Systems.Scenes;
using Fletch.Hosting.Abstractions.Contexts;
using Fletch.Hosting.API;
using Fletch.Input.API;
using Fletch.Platform.Abstractions.Paths;
using Fletch.Rendering.API;

namespace Fletch.Hosting.Contexts
{
    internal class FletchGameContext : IGameContext
    {
        public SceneManager SceneManager { get; }

        public RenderingResources Rendering { get; }

        public InputService Input { get; }

        public IPathProvider PathProvider { get; }

        public ApplicationLifetime ApplicationLifetime { get; }

        public FletchGameContext(SceneManager sceneManager, RenderingResources rendering, InputService input, IPathProvider pathProvider, ApplicationLifetime applicationLifetime)
        {
            SceneManager = sceneManager;
            Rendering = rendering;
            Input = input;
            PathProvider = pathProvider;
            ApplicationLifetime = applicationLifetime;
        }
    }
}
