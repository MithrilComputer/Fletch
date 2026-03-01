using Fletch.Core.Colors;
using Fletch.Core.Diagnostics;
using Fletch.Core.EngineConfig;
using Fletch.Core.LifeCycle;
using Fletch.Core.Math.Geometry;
using Fletch.Core.Platform;
using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Model;
using Fletch.Engine.Scenes;
using Fletch.Engine.Systems;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Rendering.Abstractions.Cameras;
using Fletch.Rendering.Abstractions.Managers;
using Fletch.Rendering.Components;
using System.Numerics;

namespace Fletch.Rendering.Systems
{
    internal sealed class WorldRenderingSystem : SceneSubsystem, IRenderable
    {
        private readonly IRenderingBackend renderingBackend;

        private readonly IFletchContextLogger<WorldRenderingSystem> logger;

        private readonly TrackedSet<SpriteRenderer> spriteRenderers = new TrackedSet<SpriteRenderer>();

        private readonly ICameraManager cameraManager;

        private readonly IRenderSurface screenSurface;

        bool spriteOrderDirty = true;

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

            scene.SystemManager.RegisterSystem(this, SystemExecutionOrder.Rendering, 0);

            scene.AddSystemComponentRegistration(typeof(SpriteRenderer), (b, c) => OnSpriteRendererChange(b, c));
            scene.AddSystemComponentRegistration(typeof(Camera2D), (b, c) => OnCamera2DChange(b, c));
        }

        public void Render(FrameTime frameTime)
        {
            //TODO, reduce complexity
            //TODO, Add Time Alpha stuff

            cameraManager.FlushSafePoint();
            spriteRenderers.Refresh();

            if (!screenSurface.IsValid)
                return;

            if(spriteOrderDirty)
            {
                spriteRenderers.Sort(SystemCompare);
                spriteOrderDirty = false;
            }

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

                backendCamera.SetZoom(frontendCamera.Zoom); // Replace later
                backendCamera.SetPosition(frontendCamera.Position);

                foreach (SpriteRenderer sprite in spriteRenderers.Items)
                {
                    if (!sprite.IsEnabled || sprite.VisualResource.Texture == null)
                        continue;

                    if(!IsSpriteVisible(visibleArea, sprite))
                        continue;

                    float spritePPU = sprite.VisualResource.PixelPerWorldUnit;
                    float worldUnitsPerPixelForThisSprite = 1.0f / spritePPU;

                    Vector2 drawScale =
                        sprite.GameObject.Transform.WorldScale *
                        sprite.ScaleOffset *
                        worldUnitsPerPixelForThisSprite;

                    Vector2 drawPosition = sprite.GameObject.Transform.WorldPosition + sprite.PositionOffset;
                    float drawRotation = sprite.GameObject.Transform.WorldRotation.Radians + sprite.RotationOffset.Radians;

                    renderingBackend.SpriteBatcher.Draw(
                        texture: sprite.VisualResource.Texture,
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
            if (sprite.VisualResource.Texture == null)
                return false;

            RectangleFloat spriteBounds = GetSpriteWorldBounds(sprite);
            return worldArea.Intersects(spriteBounds);
        }

        private static RectangleFloat GetSpriteWorldBounds(SpriteRenderer sprite)
        {
            float pixelWidth = sprite.SourceRectangle?.Width ?? sprite.VisualResource.Texture!.Width;
            float pixelHeight = sprite.SourceRectangle?.Height ?? sprite.VisualResource.Texture!.Height;

            float importPixelsPerUnit = sprite.VisualResource.PixelPerWorldUnit; //TODO change this to use a dynamic thingy
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

            if (MathF.Abs(rotation) > 0.0001f)
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

        private void OnSpriteRendererChange(GameObjectComponent component, ComponentChangeType changeType)
        {
            if (component is not SpriteRenderer sprite)
            {
                throw new InvalidOperationException($"Expected component of type {typeof(SpriteRenderer)}, but got {component.GetType()}.");
            }

            switch (changeType)
            {
                case ComponentChangeType.Added:
                    spriteRenderers.MarkToAdd(sprite);
                    spriteOrderDirty = true;

                    sprite.ZIndex = zSpriteIndex;
                    zSpriteIndex++;

                    break;
                case ComponentChangeType.Removed:
                    spriteRenderers.MarkToRemove(sprite);
                    spriteOrderDirty = true;
                    break;

                case ComponentChangeType.Modified:
                    spriteOrderDirty = true;
                    break;

                default: break;
            }
        }

        private void OnCamera2DChange(GameObjectComponent component, ComponentChangeType changeType)
        {
            if(component is not Camera2D cameraComponent)
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
