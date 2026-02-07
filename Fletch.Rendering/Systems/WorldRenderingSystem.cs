using Fletch.Core.Components.Update;
using Fletch.Core.Diagnostics;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy;
using Fletch.Engine.Model;
using Fletch.Engine.Scenes;
using Fletch.Engine.Systems;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Rendering.Abstractions.Cameras;
using Fletch.Rendering.Abstractions.Managers;
using Fletch.Rendering.Components;
using Fletch.Rendering.Model;
using Fletch.Core.Math.Geometry;

using Fletch.Core.Platform;
using System.Numerics;

namespace Fletch.Rendering.Systems
{
    internal class WorldRenderingSystem : SceneSubsystem, IUpdateable
    {
        private readonly IRenderingBackend renderingBackend;

        private readonly IFletchContextLogger<WorldRenderingSystem> logger;

        private readonly TrackedSet<SpriteRenderer> spriteRenderers = new TrackedSet<SpriteRenderer>();

        private readonly ICameraManager cameraManager;

        private readonly IRenderSurface screenSurface;

        //TODO Figure out how to get cameras to work, working on viewport stuff now

        public Color ClearColor { get; set; } = Color.Gray;

        public WorldRenderingSystem(IFletchContextLogger<WorldRenderingSystem> logger, IRenderingBackend renderingBackend, ICameraManager cameraManager, IRenderSurface screenSurface)
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

            foreach (Camera2D frontendCamera in cameraManager.Cameras)
            {
                if(!frontendCamera.IsEnabled || !screenSurface.IsValid)
                    continue;

                ICamera? backendCamera = cameraManager.GetBackendCameraFromBinding(frontendCamera);

                if (backendCamera == null)
                {
                    throw new InvalidOperationException(
                        $"Camera2D has no backend binding. " +
                        $"GameObject='{frontendCamera.GameObject?.Name ?? "<null>"}', " +
                        $"IsMain={frontendCamera.IsMainCamera}.");
                }

                if (frontendCamera.IsMainCamera)
                {
                    renderingBackend.BeginCamera(
                    backendCamera,
                    new RectangleInt(0, 0, screenSurface.Width, screenSurface.Height),
                    frontendCamera.BlendMode,
                    frontendCamera.SamplerMode);
                } 
                else
                {
                    renderingBackend.BeginCamera(
                    backendCamera,
                    frontendCamera.Viewport,
                    frontendCamera.BlendMode,
                    frontendCamera.SamplerMode);
                }

                foreach (SpriteRenderer sprite in spriteRenderers.Items)
                {
                    if (!sprite.IsEnabled || sprite.Texture == null)
                        continue;

                    Vector2 drawPosition = sprite.GameObject.Transform.WorldPosition + sprite.PositionOffset;
                    Vector2 drawScale = sprite.GameObject.Transform.WorldScale * sprite.ScaleOffset;
                    float drawRotation = sprite.GameObject.Transform.WorldRotation.Radians + sprite.RotationOffset.Radians;

                    renderingBackend.SpriteBatcher.Draw(
                        texture: sprite.Texture,
                        sourceRectangle: sprite.SourceRectangle,
                        position: drawPosition,
                        rotation: drawRotation,
                        scale: drawScale,
                        color: sprite.Color,
                        origin: sprite.Origin, // TODO Later add a origin in local space to the transform
                        layerDepth: sprite.ZIndex, //TODO add some form of list sorting later, tracked set plus order index should be fine enough.
                        spriteEffect: sprite.Effect
                        ); //TODO get the texture
                }

                renderingBackend.EndCamera();
            }
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
                    cameraManager.QueueCreate(cameraComponent);
                    break;
                case ComponentChangeType.Removed:
                    cameraManager.QueueRemove(cameraComponent);
                    break;

                default: break;
            }
        }
    }
}
