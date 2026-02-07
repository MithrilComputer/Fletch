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
using Fletch.Core.EngineConfig;

namespace Fletch.Rendering.Systems
{
    internal sealed class WorldRenderingSystem : SceneSubsystem, IUpdateable
    {
        private readonly IRenderingBackend renderingBackend;

        private readonly IFletchContextLogger<WorldRenderingSystem> logger;

        private readonly TrackedSet<SpriteRenderer> spriteRenderers = new TrackedSet<SpriteRenderer>();

        private readonly ICameraManager cameraManager;

        private readonly IRenderSurface screenSurface;

        private int zSpriteIndex = 0;

        public Color ClearColor { get; set; } = Color.Gray;

        public WorldRenderingSystem(IFletchContextLogger<WorldRenderingSystem> logger, IRenderingBackend renderingBackend, ICameraManager cameraManager, IRenderSurface screenSurface)
        {
            this.logger = logger;
            this.renderingBackend = renderingBackend;
            this.cameraManager = cameraManager;
            this.screenSurface = screenSurface;
        }

        public override void AttachToScene(Scene scene)
        {
            base.AttachToScene(scene);

            scene.SystemScheduler.RegisterSystem(this, SystemExecutionOrder.Rendering, 0);

            scene.AddSystemComponentRegistration(typeof(SpriteRenderer), (b, c) => OnSpriteRendererChange(b, c));
            scene.AddSystemComponentRegistration(typeof(Camera2D), (b, c) => OnCamera2DChange(b, c));
        }

        public void Update(float deltaTime)
        {
            cameraManager.FlushSafePoint();
            spriteRenderers.Refresh();

            if (!screenSurface.IsValid)
                return;

            spriteRenderers.Sort(SystemCompare);

            foreach (Camera2D frontendCamera in cameraManager.Cameras)
            {
                if(!frontendCamera.IsEnabled)
                    continue;

                ICamera? backendCamera = cameraManager.GetBackendCameraFromBinding(frontendCamera);

                if (backendCamera == null)
                {
                    throw new InvalidOperationException(
                        $"Camera2D has no backend binding. " +
                        $"GameObject='{frontendCamera.GameObject?.Name ?? "<null>"}', " +
                        $"IsMain={frontendCamera.IsMainCamera}.");
                }

                RectangleFloat visibleArea = backendCamera.GetVisibleArea();

                if (frontendCamera.IsMainCamera)
                {
                    renderingBackend.BeginCamera(
                        backendCamera,
                        new RectangleInt(0, 0, screenSurface.Width, screenSurface.Height),
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

                foreach (SpriteRenderer sprite in spriteRenderers.Items)
                {
                    if (!sprite.IsEnabled || sprite.Texture == null)
                        continue;

                    if(!IsSpriteVisible(visibleArea, sprite))
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
                        origin: sprite.Origin,
                        layerDepth: sprite.ZHeight,
                        spriteEffect: sprite.Effect
                    );
                }

                renderingBackend.EndCamera();
            }
        }

        private static bool IsSpriteVisible(RectangleFloat worldArea, SpriteRenderer sprite)
        {
            if (sprite.Texture == null)
                return false;

            RectangleFloat spriteBounds = GetSpriteWorldBounds(sprite);
            return worldArea.Intersects(spriteBounds);
        }

        private static RectangleFloat GetSpriteWorldBounds(SpriteRenderer sprite)
        {
            float pixelWidth = sprite.SourceRectangle?.Width ?? sprite.Texture!.Width;
            float pixelHeight = sprite.SourceRectangle?.Height ?? sprite.Texture!.Height;

            float importPixelsPerUnit = EngineConfig.TemporaryImportPPU; //TODO change this to use a dynamic thingy
            float widthUnits = pixelWidth / importPixelsPerUnit;
            float heightUnits = pixelHeight / importPixelsPerUnit;

            Vector2 worldPosition = sprite.GameObject.Transform.WorldPosition;
            Vector2 worldScale = sprite.GameObject.Transform.WorldScale;

            Vector2 finalScale = worldScale * sprite.ScaleOffset;

            float scaledWidth = MathF.Abs(widthUnits * finalScale.X);
            float scaledHeight = MathF.Abs(heightUnits * finalScale.Y);

            Vector2 p = worldPosition + sprite.PositionOffset;

            float originX = scaledWidth * sprite.Origin.X;
            float originY = scaledHeight * sprite.Origin.Y;

            float rotation =
                sprite.GameObject.Transform.WorldRotation.Radians +
                sprite.RotationOffset.Radians;

            if (rotation != 0f)
            {
                float cos = MathF.Abs(MathF.Cos(rotation));
                float sin = MathF.Abs(MathF.Sin(rotation));

                float aabbWidth = scaledWidth * cos + scaledHeight * sin;
                float aabbHeight = scaledWidth * sin + scaledHeight * cos;

                float aabbOriginX = aabbWidth * sprite.Origin.X;
                float aabbOriginY = aabbHeight * sprite.Origin.Y;

                return new RectangleFloat(
                    p.X - aabbOriginX,
                    p.Y - aabbOriginY,
                    aabbWidth,
                    aabbHeight
                );
            }

            return new RectangleFloat(
                p.X - originX,
                p.Y - originY,
                scaledWidth,
                scaledHeight
            );
        }

        private static int SystemCompare(SpriteRenderer a, SpriteRenderer b)
        {
            int order = a.ZHeight.CompareTo(b.ZHeight);
            if (order != 0)
                return order;

            return a.ZIndex.CompareTo(b.ZIndex);
        }

        private void OnSpriteRendererChange(Component component, ComponentChangeType changeType)
        {
            SpriteRenderer sprite = (SpriteRenderer)component;

            switch (changeType)
            {
                case ComponentChangeType.Added:
                    spriteRenderers.MarkToAdd(sprite);

                    sprite.ZIndex = zSpriteIndex;
                    zSpriteIndex++;

                    break;
                case ComponentChangeType.Removed:
                    spriteRenderers.MarkToRemove(sprite);
                    break;

                default: break;
            }
        }

        private void OnCamera2DChange(Component component, ComponentChangeType changeType)
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
