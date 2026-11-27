using Fletch.Core.Math.Geometry;
using Fletch.Engine.Model;
using Fletch.Rendering.Abstractions.Drawing;
using Fletch.Rendering.Components;
using Fletch.Rendering.Systems;
using System.Numerics;

namespace Fletch.Rendering.Managers
{
    internal class WorldSpriteManager : IDisposable
    {
        private readonly TrackedSet<WorldTextRenderer> textRenderers = new TrackedSet<WorldTextRenderer>();
        private readonly TrackedSet<SpriteRenderer> spriteRenderers = new TrackedSet<SpriteRenderer>();

        private bool isDisposed = false;

        public WorldSpriteManager() {}  

        public void RegisterRenderer(WorldTextRenderer textRenderer)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(WorldSpriteManager));

            textRenderers.MarkToAdd(textRenderer);
        }

        public void RegisterRenderer(SpriteRenderer spriteRenderer)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(WorldSpriteManager));

            spriteRenderers.MarkToAdd(spriteRenderer);
        }

        public void UnregisterRenderer(WorldTextRenderer textRenderer)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(WorldSpriteManager));

            textRenderers.MarkToRemove(textRenderer);
        }

        public void UnregisterRenderer(SpriteRenderer spriteRenderer)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(WorldSpriteManager));

            spriteRenderers.MarkToRemove(spriteRenderer);
        }

        public void Refresh()
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(WorldSpriteManager));

            textRenderers.Refresh();
            spriteRenderers.Refresh();
        }

        public void RenderAll(ISpriteBatcher batcher, RectangleFloat visibleArea)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(WorldSpriteManager));

            RenderSprites(batcher, visibleArea);
            RenderTexts(batcher, visibleArea);
        }

        private void RenderSprites(ISpriteBatcher batcher, RectangleFloat visibleArea)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(WorldSpriteManager));

            foreach (SpriteRenderer sprite in spriteRenderers.Items)
            {
                if (!sprite.IsEnabled || sprite.VisualResource.Texture == null || sprite.GameObject == null)
                    continue;

                float spritePPU = sprite.VisualResource.PixelPerWorldUnit;

                Vector2 scale =
                    sprite.GameObject.Transform.WorldScale *
                    sprite.ScaleOffset;

                Vector2 position =
                    sprite.RenderPose.Position + sprite.PositionOffset;

                float rotation =
                    sprite.RenderPose.Rotation + sprite.RotationOffset.Radians;

                Vector2 size = new Vector2(
                    sprite.SourceRectangle?.Width ?? sprite.VisualResource.Texture.Width,
                    sprite.SourceRectangle?.Height ?? sprite.VisualResource.Texture.Height
                );

                if (!IsVisible(
                    visibleArea,
                    size,
                    spritePPU,
                    position,
                    scale,
                    sprite.Origin,
                    rotation))
                    continue;

                Vector2 drawScale = scale / spritePPU;

                float worldUnitsPerPixelForThisSprite = 1.0f / spritePPU;

                Vector2 drawPosition = sprite.RenderPose.Position + sprite.PositionOffset;
                float drawRotation = sprite.RenderPose.Rotation + sprite.RotationOffset.Radians;

                batcher.Draw(
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
        }


        private void RenderTexts(ISpriteBatcher batcher, RectangleFloat visibleArea)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(WorldSpriteManager));

            foreach (WorldTextRenderer textRenderer in textRenderers.Items)
            {
                if (!textRenderer.IsEnabled ||
                    textRenderer.Font == null ||
                    textRenderer.GameObject == null ||
                    string.IsNullOrEmpty(textRenderer.Text))
                    continue;

                Vector2 drawPosition =
                    textRenderer.RenderPose.Position +
                    textRenderer.PositionOffset;

                float drawRotation =
                    textRenderer.RenderPose.Rotation +
                    textRenderer.RotationOffset.Radians;

                Vector2 worldScale =
                    textRenderer.GameObject.Transform.WorldScale *
                    textRenderer.ScaleOffset;

                float fontScale =
                    textRenderer.FontSize / textRenderer.Font.NominalSize;

                Vector2 drawScale = worldScale * fontScale;

                if (!IsVisible(
                    visibleArea,
                    textRenderer.LocalSize,
                    1f,
                    drawPosition,
                    worldScale,
                    textRenderer.Origin,
                    drawRotation))
                    continue;

                batcher.DrawString(
                    font: textRenderer.Font,
                    text: textRenderer.Text,
                    position: drawPosition,
                    rotation: drawRotation,
                    scale: drawScale,
                    color: textRenderer.Color,
                    origin: textRenderer.Origin,
                    layerDepth: textRenderer.ZHeight
                );
            }
        }


        private static bool IsVisible(
            RectangleFloat visibleArea,
            Vector2 pixelSize,
            float pixelsPerWorldUnit,
            Vector2 position,
            Vector2 scale,
            Vector2 origin,
            float rotation)
        {
            return visibleArea.Intersects(GetWorldBounds(
                pixelSize,
                pixelsPerWorldUnit,
                position,
                scale,
                origin,
                rotation
            ));
        }

        internal static RectangleFloat GetWorldBounds(
            Vector2 pixelSize,
            float pixelsPerWorldUnit,
            Vector2 position,
            Vector2 scale,
            Vector2 origin,
            float rotation)
        {
            Vector2 size = pixelSize / pixelsPerWorldUnit * scale;

            Vector2 centerOffset = size * (new Vector2(0.5f) - origin);
            Vector2 halfSize = Vector2.Abs(size) * 0.5f;

            float cos = MathF.Cos(rotation);
            float sin = MathF.Sin(rotation);

            Vector2 center = position + new Vector2(
                centerOffset.X * cos - centerOffset.Y * sin,
                centerOffset.X * sin + centerOffset.Y * cos
            );

            Vector2 extents = new Vector2(
                halfSize.X * MathF.Abs(cos) + halfSize.Y * MathF.Abs(sin),
                halfSize.X * MathF.Abs(sin) + halfSize.Y * MathF.Abs(cos)
            );

            return new RectangleFloat(
                center.X - extents.X,
                center.Y - extents.Y,
                extents.X * 2f,
                extents.Y * 2f
            );
        }

        public void Dispose()
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(WorldSpriteManager));

            isDisposed = true;

            spriteRenderers.Clear();

            textRenderers.Clear();
        }
    }
}
