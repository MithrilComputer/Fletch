using Fletch.Core.Components.Update;
using Fletch.Core.Diagnostics;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy;
using Fletch.Engine.Model;
using Fletch.Engine.Scenes;
using Fletch.Engine.Systems;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Rendering.Components;

namespace Fletch.Rendering.Systems
{
    internal class WorldRenderingSystem : SceneSubsystem, IUpdateable
    {
        public readonly IRenderingBackend renderingBackend;

        public readonly IFletchContextLogger<WorldRenderingSystem> logger;

        private readonly Queue<SpriteRenderer> pendingSpriteAdds = new Queue<SpriteRenderer>();
        private readonly Queue<SpriteRenderer> pendingSpriteRemoves = new Queue<SpriteRenderer>();

        private readonly Queue<Camera2D> pendingCameraAdds = new Queue<Camera2D>();
        private readonly Queue<Camera2D> pendingCameraRemoves = new Queue<Camera2D>();

        private readonly List<Camera2D> cameras = new List<Camera2D>();
        private readonly List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

        public WorldRenderingSystem(IFletchContextLogger<WorldRenderingSystem> logger, IRenderingBackend renderingBackend)
        {
            this.renderingBackend = renderingBackend;
            this.logger = logger;
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
            
        }

        private void OnSpriteRendererChange(GameObject gameObject, Component component, ComponentChangeType changeType)
        {
            
        }

        private void OnCamera2DChange(GameObject gameObject, Component component, ComponentChangeType changeType)
        {

        }
    }
}
