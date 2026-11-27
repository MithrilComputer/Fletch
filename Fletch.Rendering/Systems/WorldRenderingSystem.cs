using Fletch.Core.Colors;
using Fletch.Core.Diagnostics;
using Fletch.Core.EngineConfig;
using Fletch.Core.LifeCycle;
using Fletch.Core.Math.Geometry;
using Fletch.Core.Platform;
using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Model;
using Fletch.Engine.Systems;
using Fletch.Engine.Systems.Scenes;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Rendering.Abstractions.Cameras;
using Fletch.Rendering.Abstractions.Managers;
using Fletch.Rendering.Components;
using Fletch.Rendering.Managers;

namespace Fletch.Rendering.Systems
{
    internal sealed class WorldRenderingSystem : SceneSubsystem, IRenderable, IDisposable
    {
        private readonly IRenderingBackend renderingBackend;

        private readonly IFletchContextLogger<WorldRenderingSystem> logger;

        private readonly WorldSpriteManager worldSpriteManager = new WorldSpriteManager();

        private readonly ICameraManager cameraManager;

        private readonly IRenderSurface screenSurface;

        private bool isDisposed = false;

        private int zSpriteIndex = 0;

        public WorldRenderingSystem(IFletchContextLogger<WorldRenderingSystem> logger, IRenderingBackend renderingBackend, ICameraManager cameraManager, IRenderSurface screenSurface)
        {
            this.logger = logger;
            this.renderingBackend = renderingBackend;
            this.cameraManager = cameraManager;
            this.screenSurface = screenSurface;
        }

        public override void AttachToScene(Scene scene)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(WorldRenderingSystem));

            base.AttachToScene(scene);

            scene.SystemManager.RegisterSystem(this, SystemExecutionOrder.Rendering, 0);

            scene.AddSystemComponentRegistration(typeof(SpriteRenderer), OnSpriteRendererChange);
            scene.AddSystemComponentRegistration(typeof(WorldTextRenderer), OnWorldTextRendererChange);
        }

        public void Render(FrameTime frameTime)
        {
            if(isDisposed)
                throw new ObjectDisposedException(nameof(WorldRenderingSystem));

            worldSpriteManager.Refresh();

            if (!screenSurface.IsValid)
                return;

            for (int i = 0; i < cameraManager.Cameras.Count; i++)
            {
                Camera2D frontendCamera = cameraManager.Cameras[i];

                if (!frontendCamera.IsEnabled)
                    continue;

                ICamera? backendCamera = cameraManager.GetBackendCameraFromBinding(frontendCamera);

                if (backendCamera == null)
                {
                    throw new InvalidOperationException(
                        $"Camera2D has no backend binding. " +
                        $"GameObject='{frontendCamera.GameObject?.Name ?? "<null>"}', " +
                        $"IsMain={frontendCamera.IsMainCamera}.");
                }

                backendCamera.SetZoom(frontendCamera.Zoom);
                backendCamera.SetPosition(frontendCamera.Position);

                RectangleFloat visibleArea = backendCamera.GetVisibleArea();

                if (frontendCamera.IsMainCamera || frontendCamera.Viewport.Equals(RectangleInt.Zero))
                {
                    renderingBackend.BeginCamera(
                        backendCamera,
                        new RectangleInt(0, 0, EngineConfig.VirtualResolution.X, EngineConfig.VirtualResolution.Y),
                        frontendCamera.BlendMode,
                        frontendCamera.SamplerMode
                    );
                }
                else
                {
                    renderingBackend.BeginCamera(
                        backendCamera,
                        frontendCamera.Viewport,
                        frontendCamera.BlendMode,
                        frontendCamera.SamplerMode
                    );
                }

                worldSpriteManager.RenderAll(renderingBackend.SpriteBatcher, visibleArea);

                renderingBackend.EndCamera();
            }
        }

        private void OnSpriteRendererChange(Component component, ComponentChangeType changeType)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(WorldRenderingSystem));

            if (component is not SpriteRenderer sprite)
            {
                throw new InvalidOperationException($"Expected component of type {typeof(SpriteRenderer)}, but got {component.GetType()}.");
            }

            switch (changeType)
            {
                case ComponentChangeType.Added:
                    worldSpriteManager.RegisterRenderer(sprite);

                    break;

                case ComponentChangeType.Removed:
                    worldSpriteManager.UnregisterRenderer(sprite);
                    break;

                case ComponentChangeType.Modified:
                    break;

                default: break;
            }
        }

        private void OnWorldTextRendererChange(Component component, ComponentChangeType changeType)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(WorldRenderingSystem));

            if (component is not WorldTextRenderer textRenderer)
            {
                throw new InvalidOperationException($"Expected component of type {typeof(WorldTextRenderer)}, but got {component.GetType()}.");
            }

            switch (changeType)
            {
                case ComponentChangeType.Added:
                    worldSpriteManager.RegisterRenderer(textRenderer);
                    break;

                case ComponentChangeType.Removed:
                    worldSpriteManager.UnregisterRenderer(textRenderer);
                    break;

                case ComponentChangeType.Modified:
                    
                    break;

                default: break;
            }
        }

        public void Dispose()
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(WorldRenderingSystem));

            isDisposed = true;

            worldSpriteManager.Dispose();
        }
    }
}
