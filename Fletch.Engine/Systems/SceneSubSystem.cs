using Fletch.Engine.Scenes;

namespace Fletch.Engine.Systems
{
    internal abstract class SceneSubsystem
    {
        public virtual int Order => 0;

        public Scene? Scene { get; private set; }

        public virtual void AttachToScene(Scene scene)
        {
            Scene = scene;
        }

        public virtual void DetachFromScene()
        {
            Scene = null;
        }
    }
}
