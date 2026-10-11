using Fletch.Engine.Systems.Scenes;
using Fletch.Input.Abstractions.Backends;
using Fletch.Rendering.Abstractions.Backends;

namespace Fletch.Hosting.Abstractions.Contexts
{
    public interface IGameContext
    {
        SceneManager SceneManager { get; }

        IRenderingBackend RenderingBackend { get; }

        IInputBackend InputBackend { get; }
    }
}
