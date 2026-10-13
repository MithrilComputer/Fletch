using Fletch.Engine.Systems.Scenes;
using Fletch.Hosting.API;
using Fletch.Input.API;
using Fletch.Platform.Abstractions.Paths;
using Fletch.Rendering.API;

namespace Fletch.Hosting.Abstractions.Contexts
{
    public interface IGameContext
    {
        SceneManager SceneManager { get; }

        RenderingResources Rendering { get; }

        InputService Input { get; }

        IPathProvider PathProvider { get; }

        ApplicationLifetime ApplicationLifetime { get; }

        //TODO Physics, raycasts, ect
    }
}
