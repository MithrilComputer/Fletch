using Fletch.Core.LifeCycle;
using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Model;
using Fletch.Engine.Systems;
using Fletch.Engine.Systems.Scenes;
using Fletch.Rendering.Abstractions.Managers;
using Fletch.Rendering.Components;

namespace Fletch.Rendering.Systems
{
    internal class CameraSystem : SceneSubsystem, IRenderable
    {
        private readonly ICameraManager cameraManager;

        public CameraSystem(ICameraManager cameraManager)
        {
            this.cameraManager = cameraManager;
        }

        public override void AttachToScene(Scene scene)
        {
            base.AttachToScene(scene);

            scene.SystemManager.RegisterSystem(this, SystemExecutionOrder.RenderPrep, 0);

            scene.AddSystemComponentRegistration(typeof(Camera2D), OnCamera2DChange);
        }

        public void Render(FrameTime frameTime)
        {
            cameraManager.FlushSafePoint();
        }

        private void OnCamera2DChange(GameObjectComponent component, ComponentChangeType changeType)
        {
            if (component is not Camera2D cameraComponent)
            {
                throw new InvalidOperationException($"Expected component of type {typeof(Camera2D)}, but got {component.GetType()}.");
            }

            switch (changeType)
            {
                case ComponentChangeType.Added:
                    cameraManager.QueueCreate(cameraComponent);

                    break;
                case ComponentChangeType.Removed:
                    cameraManager.QueueRemove(cameraComponent);
                    break;

                case ComponentChangeType.Modified:
                    cameraManager.MarkDirty();
                    break;

                default: break;
            }
        }
    }
}
