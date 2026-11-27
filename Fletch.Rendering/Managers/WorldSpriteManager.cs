using Fletch.Core.Math.Geometry;
using Fletch.Engine.Model;
using Fletch.Rendering.Abstractions.Drawing;
using Fletch.Rendering.Components;
using System.Numerics;

namespace Fletch.Rendering.Managers
{
    internal class WorldSpriteManager
    {
        private readonly TrackedSet<WorldTextRenderer> textRenderers = new TrackedSet<WorldTextRenderer>();
        private readonly TrackedSet<SpriteRenderer> spriteRenderers = new TrackedSet<SpriteRenderer>();

        public WorldSpriteManager() {}  

        public void RegisterRenderer(WorldTextRenderer textRenderer)
        {
            textRenderers.MarkToAdd(textRenderer);
        }

        public void RegisterRenderer(SpriteRenderer spriteRenderer)
        {
            spriteRenderers.MarkToAdd(spriteRenderer);
        }

        public void UnregisterRenderer(WorldTextRenderer textRenderer)
        {
            textRenderers.MarkToRemove(textRenderer);
        }

        public void UnregisterRenderer(SpriteRenderer spriteRenderer)
        {
            spriteRenderers.MarkToRemove(spriteRenderer);
        }

        public void Refresh()
        {
            textRenderers.Refresh();
            spriteRenderers.Refresh();
            
        }

        public void RenderAll(ISpriteBatcher batcher, RectangleFloat visibleArea)
        {
            RenderSprites(batcher, visibleArea);
            //RenderTexts(batcher, visibleArea);
        }

        private void RenderSprites(ISpriteBatcher batcher, RectangleFloat visibleArea)
        {
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
                    layerDepth: 0,
                    spriteEffect: sprite.Effect
                );
            }
        }

        /*
        private void RenderTexts(ISpriteBatcher batcher, RectangleFloat visibleArea)
        {
            foreach (WorldTextRenderer textRenderer in textRenderers.Items)
            {
                if (!textRenderer.IsEnabled || textRenderer.VisualResource.Texture == null || textRenderer.GameObject == null)
                    continue;

                if (!IsVisible(
                    visibleArea, 
                    textRenderer.VisualResource.Texture.Size,
                    textRenderer.VisualResource.PixelPerWorldUnit,
                    textRenderer.RenderPose.Position,
                    textRenderer.GameObject.Transform.WorldScale,
                    textRenderer.Origin,
                    textRenderer.RenderPose.Rotation))
                    continue;

                float spritePPU = textRenderer.VisualResource.PixelPerWorldUnit;
                float worldUnitsPerPixelForThisSprite = 1.0f / spritePPU;

                Vector2 drawScale =
                    textRenderer.GameObject.Transform.WorldScale *
                    textRenderer.ScaleOffset *
                    worldUnitsPerPixelForThisSprite;

                Vector2 drawPosition = textRenderer.RenderPose.Position + textRenderer.PositionOffset;
                float drawRotation = textRenderer.RenderPose.Rotation + textRenderer.RotationOffset.Radians;

                batcher.Draw(
                    texture: textRenderer.VisualResource.Texture,
                    sourceRectangle: textRenderer.SourceRectangle,
                    position: drawPosition,
                    rotation: drawRotation,
                    scale: drawScale,
                    color: textRenderer.Color,
                    origin: textRenderer.Origin,
                    layerDepth: 0,
                    spriteEffect: textRenderer.Effect
                );
            }
        }
        */

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
    }
}
