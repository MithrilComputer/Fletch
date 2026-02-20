using Fletch.Core.Colors;
using Fletch.Core.Math.Geometry;
using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.Model;
using System.Numerics;

namespace Fletch.Rendering.Abstractions.Drawing
{
    internal interface ISpriteBatcher
    {
        /// <summary>
        /// If a sprite batch is currently open between a <see cref="Begin"/> and <see cref="End"/> call.
        /// </summary>
        bool IsBatchOpen { get; }

        /// <summary>
        /// Begins a new sprite batch.
        /// </summary>
        /// <param name="transformMatrix">
        /// The transform applied to all sprites in this batch, typically your camera matrix.
        /// </param>
        /// <param name="blendMode">
        /// The blending mode used when drawing sprites in this batch. Alpha is the default.
        /// </param>
        /// <param name="samplerMode">
        /// The texture sampling mode used when scaling textures (linear or point).
        /// </param>
        void Begin(Matrix3x2 transformMatrix, BlendMode blendMode = BlendMode.Alpha, SamplerMode samplerMode = SamplerMode.Linear);

        /// <summary>
        /// Ends the currently active sprite batch and flushes all queued draw calls to the GPU.
        /// </summary>
        void End();

        #pragma warning disable S107 // Methods has many parameters by design

        /// <summary>
        /// Draws a textured sprite to the active sprite batch.
        /// </summary>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="position">The world-space position of the sprite’s origin.</param>
        /// <param name="sourceRectangle">The portion of the texture to draw, or null for full texture.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="rotation">Rotation of the sprite in radians.</param>
        /// <param name="origin">Rotation/scaling origin in texture pixel coordinates.</param>
        /// <param name="scale">Non-uniform scale applied on X and Y axes.</param>
        /// <param name="layerDepth">Sorting depth of the sprite within the current batch.</param>
        /// <param name="spriteEffect">Optional flip effect.</param>
        void Draw(
            ITexture texture,
            Vector2 position,
            RectangleFloat? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            float layerDepth,
            SpriteEffect spriteEffect = SpriteEffect.None);

        /// <summary>
        /// Draws a texture at the given position with a color tint.
        /// </summary>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="position">The world-space position of the sprite.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="spriteEffect">Optional flip effect.</param>
        void Draw(
            ITexture texture,
            Vector2 position,
            Color color,
            SpriteEffect spriteEffect = SpriteEffect.None);

        /// <summary>
        /// Draws a texture at the given position with a color tint and explicit layer depth.
        /// </summary>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="position">The world-space position of the sprite.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="layerDepth">The depth sorting value within the current sprite batch.</param>
        /// <param name="spriteEffect">Optional flip effect.</param>
        void Draw(
            ITexture texture,
            Vector2 position,
            Color color,
            float layerDepth,
            SpriteEffect spriteEffect = SpriteEffect.None);

        /// <summary>
        /// Draws a sub-region of a texture at the given position.
        /// </summary>
        /// <param name="texture">The texture containing the region to draw.</param>
        /// <param name="position">The world-space position of the sprite.</param>
        /// <param name="sourceRectangle">The region of the texture to draw.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="layerDepth">Optional depth sorting value within the batch.</param>
        /// <param name="spriteEffect">Optional flip effect.</param>
        void Draw(
            ITexture texture,
            Vector2 position,
            RectangleFloat sourceRectangle,
            Color color,
            float layerDepth = 0f,
            SpriteEffect spriteEffect = SpriteEffect.None);

        /// <summary>
        /// Draws a specified region of a texture scaled to a destination rectangle.
        /// </summary>
        /// <param name="texture">The texture to draw from.</param>
        /// <param name="destinationRectangle">
        /// The rectangle that the source region will be scaled into.
        /// </param>
        /// <param name="sourceRectangle">
        /// The region of the texture to draw.
        /// </param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="layerDepth">Optional depth sorting value within the batch.</param>
        /// <param name="spriteEffect">Optional flip effect.</param>
        void Draw(
            ITexture texture,
            RectangleFloat destinationRectangle,
            RectangleFloat sourceRectangle,
            Color color,
            float layerDepth = 0f,
            SpriteEffect spriteEffect = SpriteEffect.None);

        /// <summary>
        /// Draws a texture so that it is stretched to exactly fill the specified destination rectangle.
        /// </summary>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="destinationRectangle">
        /// The rectangle in world or screen space that the texture will be stretched to fill.
        /// </param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="sourceRectangle">
        /// Optional region of the texture to draw. If null, the entire texture is used.
        /// </param>
        /// <param name="layerDepth">
        /// Depth sorting value within the current sprite batch.
        /// </param>
        /// <param name="spriteEffect">
        /// Optional sprite flip effect applied during drawing.
        /// </param>
        void DrawToRect(
            ITexture texture,
            RectangleFloat destinationRectangle,
            Color color,
            RectangleFloat? sourceRectangle = null,
            float layerDepth = 0f,
            SpriteEffect spriteEffect = SpriteEffect.None);

        /// <summary>
        /// Draws text using the specified font at the given position with a color tint.
        /// </summary>
        /// <param name="font">The font resource to use when rendering the text.</param>
        /// <param name="text">The text string to render.</param>
        /// <param name="position">
        /// The position, in pixels, where the text should be drawn
        /// (typically in screen or world space, depending on the current transform).
        /// </param>
        /// <param name="color">The color tint to apply to the rendered text.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the underlying sprite batch is not currently in a valid state to draw,
        /// or if the provided font is not compatible with the active rendering backend.
        /// </exception>
        void DrawString(
            IFont font,
            string text,
            Vector2 position,
            Color color);

        #pragma warning restore S107
    }
}
