using Fletch.Core.Math.Geometry;
using Fletch.Rendering.Abstractions.Drawing;
using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.Model;
using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using System.Numerics;
using FletchColor = Fletch.Core.Colors.Color;
using FletchSpriteEffect = Fletch.Rendering.Model.SpriteEffect;
using SystemVector = System.Numerics.Vector2;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaMatrix = Microsoft.Xna.Framework.Matrix;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using XnaSpriteEffect = Microsoft.Xna.Framework.Graphics.SpriteEffects;
using XnaVector = Microsoft.Xna.Framework.Vector2;

namespace Fletch.Rendering.MonoGame.Drawing
{
    internal sealed class MonoGameSpriteBatcher : ISpriteBatcher, IDisposable
    {
        private readonly SpriteBatch spriteBatch;

        /// <summary>
        /// If a sprite batch is currently open between a <see cref="Begin"/> and <see cref="End"/> call.
        /// </summary>
        public bool IsBatchOpen { get; private set; }

        public MonoGameSpriteBatcher(GraphicsDevice graphicsDevice)
        {
            spriteBatch = new SpriteBatch(graphicsDevice);
        }

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
        /// <exception cref="InvalidOperationException">
        /// Thrown if a batch is already open. Call <see cref="End"/> before calling <see cref="Begin"/> again.
        /// </exception>
        /// <remarks>
        /// A batch must be opened with <see cref="Begin"/> before any <see cref="Draw"/> calls can be made.
        /// Rendering state set here remains active until <see cref="End"/> is called.
        /// </remarks>
        public void Begin(Matrix3x2 transformMatrix, BlendMode blendMode = BlendMode.Alpha, SamplerMode samplerMode = SamplerMode.Linear)
        {
            if (IsBatchOpen)
                throw new InvalidOperationException("Begin called while a batch is already open. Call End() first.");

            var blendState = ConvertFromFletchType(blendMode);

            var samplerState = ConvertFromFletchType(samplerMode);

            XnaMatrix transform = new XnaMatrix(
                transformMatrix.M11, transformMatrix.M12, 0, 0,
                transformMatrix.M21, transformMatrix.M22, 0, 0,
                0, 0, 1, 0,
                transformMatrix.M31, transformMatrix.M32, 0, 1);

            spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: blendState,
                samplerState: samplerState,
                depthStencilState: DepthStencilState.None,
                rasterizerState: RasterizerState.CullNone,
                effect: null,
                transformMatrix: transform
                );

            IsBatchOpen = true;
        }

        /// <summary>
        /// Ends the currently active sprite batch and flushes all queued draw calls to the GPU.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if no batch is currently open. Call <see cref="Begin"/> before calling <see cref="End"/>.
        /// </exception>
        /// <remarks>
        /// After calling <see cref="End"/>, no more sprites may be drawn until <see cref="Begin"/> is called again.
        /// </remarks>
        public void End()
        {
            if (!IsBatchOpen)
                throw new InvalidOperationException("End called while no batch is open. Call Begin() first.");
            spriteBatch.End();
            IsBatchOpen = false;
        }

        /// <summary>
        /// Draws a textured sprite to the active sprite batch.
        /// </summary>
        /// <param name="texture">
        /// The texture to draw. Must be an <see cref="ITexture"/> instance created by this rendering backend.
        /// </param>
        /// <param name="position">
        /// The world-space position of the sprite’s origin, after rotation and scaling are applied.
        /// </param>
        /// <param name="sourceRectangle">
        /// The portion of the texture to draw. If <c>null</c>, the entire texture is used.
        /// </param>
        /// <param name="color">
        /// The color tint to apply to the sprite. Use <c>Color.White</c> to draw without tint.
        /// </param>
        /// <param name="rotation">
        /// Rotation of the sprite in radians, applied around <paramref name="origin"/>.
        /// </param>
        /// <param name="origin">
        /// The point within the sprite that acts as the rotation and scaling origin,
        /// specified in texture pixel coordinates (top-left is 0,0).
        /// </param>
        /// <param name="scale">
        /// The non-uniform scale applied to the sprite on the X and Y axes.
        /// </param>
        /// <param name="layerDepth">
        /// Sorting depth of the sprite within the current batch. Values closer to 1 are drawn on top
        /// when using SpriteSortMode.BackToFront, and closer to 0 when using FrontToBack.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if called when a batch has not been begun with <see cref="Begin"/>.
        /// </exception>
        /// <remarks>
        /// This method requires an open batch created with <see cref="Begin"/> and will throw if none is active
        /// </remarks>
#pragma warning disable S107 // Method has many parameters by design
        public void Draw(
            ITexture texture,
            SystemVector position,
            RectangleFloat? sourceRectangle,
            FletchColor color,
            float rotation,
            SystemVector origin,
            SystemVector scale,
            float layerDepth,
            FletchSpriteEffect spriteEffect = FletchSpriteEffect.None)
        {
            if (!IsBatchOpen)
                throw new InvalidOperationException("Draw called before Begin.");

            if (texture is null)
                throw new ArgumentNullException(nameof(texture));

            if (texture is not Resources.MonoGameTexture mgTexture)
                throw new InvalidOperationException("ITexture is not a MonoGame texture instance for this backend.");

            Texture2D xnaTexture = mgTexture.Texture;

            RectangleFloat rect = sourceRectangle ?? new RectangleFloat(0, 0, texture.Width, texture.Height);

            XnaRectangle xnaRectangle = ConvertFromFletchType(rect);
            XnaColor xnaColor = ConvertFromFletchType(color);
            XnaSpriteEffect effects = ConvertFromFletchType(spriteEffect) ^ XnaSpriteEffect.FlipVertically;

            var originPixels = new XnaVector(
                origin.X * xnaRectangle.Width,
                origin.Y * xnaRectangle.Height
            );

            spriteBatch.Draw(
                texture: xnaTexture,
                position: new XnaVector(position.X, position.Y),
                sourceRectangle: xnaRectangle,
                color: xnaColor,
                rotation: rotation,
                origin: new XnaVector(originPixels.X, originPixels.Y),
                scale: new XnaVector(scale.X, scale.Y),
                effects: effects,
                layerDepth: layerDepth
                );
        }

        /// <summary>
        /// Draws a texture at the given position with a color tint.
        /// </summary>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="position">The world-space position of the sprite.</param>
        /// The color tint to apply to the sprite. Use <c>Color.White</c> (the engine’s white color) to draw without tint.
        /// <remarks>
        /// This is a convenience overload that draws the entire texture with no rotation and unit scale.
        /// </remarks>
        public void Draw(ITexture texture, SystemVector position, FletchColor color,
            FletchSpriteEffect spriteEffect = FletchSpriteEffect.None)
        {
            Draw(texture,
                position,
                sourceRectangle: null,
                color: color,
                rotation: 0f,
                origin: SystemVector.Zero,
                scale: new SystemVector(1f, 1f),
                layerDepth: 0f,
                spriteEffect);
        }

        /// <summary>
        /// Draws a texture at the given position with a color tint and explicit layer depth.
        /// </summary>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="position">The world-space position of the sprite.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="layerDepth">The depth sorting value within the current sprite batch.</param>
        /// <remarks>
        /// This overload draws the full texture without rotation, using an origin of (0,0) and unit scale.
        /// </remarks>
        public void Draw(ITexture texture,
            SystemVector position,
            FletchColor color,
            float layerDepth,
            FletchSpriteEffect spriteEffect = FletchSpriteEffect.None)
        {
            Draw(texture,
                position,
                sourceRectangle: null,
                color: color,
                rotation: 0f,
                origin: SystemVector.Zero,
                scale: new SystemVector(1f, 1f),
                layerDepth: layerDepth,
                spriteEffect: spriteEffect);
        }

        /// <summary>
        /// Draws a sub-region of a texture at the given position.
        /// </summary>
        /// <param name="texture">The texture containing the region to draw.</param>
        /// <param name="position">The world-space position of the sprite.</param>
        /// <param name="sourceRectangle">
        /// The region of the texture to draw, typically used for sprite sheets or tile maps.
        /// </param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="layerDepth">Optional depth sorting value within the batch.</param>
        /// <remarks>
        /// This overload does not apply rotation and uses a unit scale.
        /// </remarks>
        public void Draw(
            ITexture texture,
            SystemVector position,
            RectangleFloat sourceRectangle,
            FletchColor color,
            float layerDepth = 0f,
            FletchSpriteEffect spriteEffect = FletchSpriteEffect.None)
        {
            Draw(texture,
                position,
                sourceRectangle,
                color,
                rotation: 0f,
                origin: SystemVector.Zero,
                scale: new SystemVector(1f, 1f),
                layerDepth: layerDepth,
                spriteEffect: spriteEffect);
        }

        /// <summary>
        /// Draws a specified region of a texture scaled to a destination rectangle.
        /// </summary>
        /// <param name="texture">The texture to draw from.</param>
        /// <param name="destinationRectangle">
        /// The rectangle, in world space, that the source region will be scaled into.
        /// </param>
        /// <param name="sourceRectangle">
        /// The region of the texture to draw, typically used for sprite sheets or UI atlases.
        /// </param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="layerDepth">Optional depth sorting value within the batch.</param>
        /// <remarks>
        /// This overload allows independently choosing a source region and destination rectangle, useful for UI and 9-slice rendering.
        /// </remarks>
        public void Draw(
            ITexture texture,
            RectangleFloat destinationRectangle,
            RectangleFloat sourceRectangle,
            FletchColor color,
            float layerDepth = 0f,
            FletchSpriteEffect spriteEffect = FletchSpriteEffect.None)
        {
            Draw(texture,
                position: new SystemVector(destinationRectangle.X, destinationRectangle.Y),
                sourceRectangle: sourceRectangle,
                color: color,
                rotation: 0f,
                origin: SystemVector.Zero,
                scale: new SystemVector(
                    destinationRectangle.Width / sourceRectangle.Width,
                    destinationRectangle.Height / sourceRectangle.Height),
                layerDepth: layerDepth,
                spriteEffect: spriteEffect);
        }

        /// <summary>
        /// Draws a texture so that it is stretched to exactly fill the specified destination rectangle.
        /// </summary>
        /// <param name="texture">
        /// The texture to draw. Must be a texture created by this rendering backend.
        /// </param>
        /// <param name="destinationRectangle">
        /// The rectangle in world or screen space that the texture will be stretched to fill.
        /// </param>
        /// <param name="color">
        /// The color tint to apply to the texture. Use white to draw without tint.
        /// </param>
        /// <param name="sourceRectangle">
        /// Optional region of the texture to draw. If null, the entire texture is used.
        /// </param>
        /// <param name="layerDepth">
        /// Depth sorting value within the current sprite batch.
        /// </param>
        /// <param name="spriteEffect">
        /// Optional sprite flip effect applied during drawing.
        /// </param>
        /// <remarks>
        /// This overload uses SpriteBatch's destination-rectangle draw path, providing
        /// pixel-perfect stretching behavior. The texture will be scaled to fill the
        /// rectangle exactly, without requiring manual scale calculations.
        /// </remarks>
        public void DrawToRect(
            ITexture texture,
            RectangleFloat destinationRectangle,
            FletchColor color,
            RectangleFloat? sourceRectangle = null,
            float layerDepth = 0f,
            FletchSpriteEffect spriteEffect = FletchSpriteEffect.None)
        {
            if (!IsBatchOpen)
                throw new InvalidOperationException("Draw called before Begin.");

            if (texture is not Resources.MonoGameTexture mgTexture)
                throw new InvalidOperationException("ITexture is not a MonoGame texture instance for this backend.");

            XnaRectangle xnaDestination = ConvertFromFletchType(destinationRectangle);
            XnaRectangle? xnaSource = sourceRectangle.HasValue ? ConvertFromFletchType(sourceRectangle.Value) : (XnaRectangle?)null;

            XnaSpriteEffect flipState = ConvertFromFletchType(spriteEffect);

            spriteBatch.Draw(
                mgTexture.Texture,
                destinationRectangle: xnaDestination,
                sourceRectangle: xnaSource,
                color: ConvertFromFletchType(color),
                rotation: 0f,
                origin: XnaVector.Zero,
                effects: flipState,
                layerDepth: layerDepth
            );
        }

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
        public void DrawString(
            IFont font,
            string text,
            SystemVector position,
            FletchColor color)
        {
            if (!IsBatchOpen)
                throw new InvalidOperationException("DrawString called before Begin.");

            if (font is not Resources.MonoGameFont monoGameFont)
                throw new InvalidOperationException("IFont is not a MonoGame font instance for this backend.");

            spriteBatch.DrawString(
                monoGameFont.SpriteFontBase,
                text,
                new XnaVector(position.X, position.Y),
                ConvertFromFletchType(color));
        }

#pragma warning restore S107

        private static BlendState ConvertFromFletchType(BlendMode blendMode)
        {
            return blendMode switch
            {
                BlendMode.Alpha => BlendState.AlphaBlend,
                BlendMode.Additive => BlendState.Additive,
                BlendMode.Opaque => BlendState.Opaque,
                _ => BlendState.AlphaBlend,
            };
        }

        private static SamplerState ConvertFromFletchType(SamplerMode samplerMode)
        {
            return samplerMode switch
            {
                SamplerMode.Linear => SamplerState.LinearClamp,
                SamplerMode.Point => SamplerState.PointClamp,
                _ => SamplerState.LinearClamp,
            };
        }

        private static XnaRectangle ConvertFromFletchType(RectangleFloat rectangle)
        {
            return new XnaRectangle(
                (int)MathF.Round(rectangle.X),
                (int)MathF.Round(rectangle.Y),
                (int)MathF.Round(rectangle.Width),
                (int)MathF.Round(rectangle.Height));
        }

        private static XnaColor ConvertFromFletchType(FletchColor fletchColor)
        {
            return new XnaColor(fletchColor.R, fletchColor.G, fletchColor.B, fletchColor.A);
        }

        private static XnaSpriteEffect ConvertFromFletchType(FletchSpriteEffect fletchSpriteEffect)
        {
            return fletchSpriteEffect switch
            {
                FletchSpriteEffect.None => XnaSpriteEffect.None,
                FletchSpriteEffect.FlipHorizontally => XnaSpriteEffect.FlipHorizontally,
                FletchSpriteEffect.FlipVertically => XnaSpriteEffect.FlipVertically,
                _ => XnaSpriteEffect.None,
            };
        }

        public void Dispose()
        {
            if (IsBatchOpen)
                spriteBatch.End();

            spriteBatch.Dispose();
        }
    }
}
