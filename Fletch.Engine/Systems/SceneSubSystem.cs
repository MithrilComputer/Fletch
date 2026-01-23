using Fletch.Engine.Scenes;

namespace Fletch.Engine.Systems
{
    public abstract class SceneSubsystem
    {
        public Scene? LoadedScene { get; private set; }

        public virtual void AttachToScene(Scene scene)
        {
            LoadedScene = scene;
        }

        public virtual void DetachFromScene()
        {
            LoadedScene = null;
        }
    }
}
