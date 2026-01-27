using Fletch.Core.Components.Update;
using Fletch.Core.Diagnostics;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy;
using Fletch.Engine.Model;
using Fletch.Engine.Scenes;
using Fletch.Engine.Systems;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Rendering.Abstractions.Managers;
using Fletch.Rendering.Components;
using Fletch.Rendering.Model;

namespace Fletch.Rendering.Systems
{
    internal class WorldRenderingSystem : SceneSubsystem, IUpdateable
    {
        public readonly IRenderingBackend renderingBackend;

        public readonly IFletchContextLogger<WorldRenderingSystem> logger;

        private readonly TrackedSet<SpriteRenderer> spriteRenderers = new TrackedSet<SpriteRenderer>();

        private readonly ICameraManager cameraManager;

        //TODO Figure out how to get cameras to work, working on viewport stuff now

        public Color ClearColor { get; set; } = Color.Gray;

        public WorldRenderingSystem(IFletchContextLogger<WorldRenderingSystem> logger, IRenderingBackend renderingBackend, ICameraManager cameraManager)
        {
            this.logger = logger;
            this.renderingBackend = renderingBackend;
            this.cameraManager = cameraManager;
        }

        public override void AttachToScene(Scene scene)
        {
            base.AttachToScene(scene);

            scene.SystemScheduler.RegisterSystem(this, SystemExecutionOrder.Rendering, 0);

            scene.AddSystemComponentRegistration(typeof(SpriteRenderer), (a, b, c) => OnSpriteRendererChange(a, b, c));
            scene.AddSystemComponentRegistration(typeof(Camera2D), (a, b, c) => OnCamera2DChange(a, b, c));
        }

        public void Update(float deltaTime)
        {
            cameraManager.FlushSafePoint();
            spriteRenderers.Refresh();
        }

        private void OnSpriteRendererChange(GameObject gameObject, Component component, ComponentChangeType changeType)
        {
            switch(changeType)
            {
                case ComponentChangeType.Added:
                    spriteRenderers.MarkToAdd((SpriteRenderer)component);
                    break;
                case ComponentChangeType.Removed:
                    spriteRenderers.MarkToRemove((SpriteRenderer)component);
                    break;

                default: break;
            }
        }

        private void OnCamera2DChange(GameObject gameObject, Component component, ComponentChangeType changeType)
        {
            Camera2D cameraComponent = (Camera2D)component;

            switch (changeType)
            {
                case ComponentChangeType.Added:
                    cameraManager.MarkCameraCreation(cameraComponent);
                    break;
                case ComponentChangeType.Removed:
                    cameraManager.MarkCameraRemoval(cameraComponent);
                    break;

                default: break;
            }
        }
    }
}
