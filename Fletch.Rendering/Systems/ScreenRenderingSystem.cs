using Fletch.Core.LifeCycle;
using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Model;
using Fletch.Engine.Systems;
using Fletch.Engine.Systems.Scenes;
using Fletch.Rendering.Components;

namespace Fletch.Rendering.Systems
{
    internal class ScreenRenderingSystem : SceneSubsystem, IRenderable
    {
        public override void AttachToScene(Scene scene)
        {
            base.AttachToScene(scene);

            scene.SystemManager.RegisterSystem(this, SystemExecutionOrder.Rendering, 0);

            scene.AddSystemComponentRegistration(typeof(SpriteRenderer), (b, c) => OnScreenSpriteRenderer(b, c));
        }

        public void Render(FrameTime frameTime)
        {
            
        }

        private void OnScreenSpriteRenderer(GameObjectComponent component, ComponentChangeType changeType)
        {
            if (component is not ScreenSpriteRenderer sprite)
            {
                throw new InvalidOperationException($"Expected component of type {typeof(SpriteRenderer)}, but got {component.GetType()}.");
            }

            switch (changeType)
            {
                case ComponentChangeType.Added:

                    break;

                case ComponentChangeType.Removed:

                    break;

                case ComponentChangeType.Modified:
                    
                    break;

                default: break;
            }
        }
    }
}
