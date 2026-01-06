using Fletch.Rendering.Abstractions.Factories;
using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.MonoGame.Resources;
using Microsoft.Xna.Framework.Graphics;
using FletchColor = Fletch.Rendering.Model.Color;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace Fletch.Rendering.MonoGame.Factories
{
    /// <summary>
    /// <para>
    /// Loads textures directly from image files on disk (PNG, JPG, etc.)
    /// without using the MonoGame content pipeline.
    /// </para>
    /// <para>
    /// This factory:
    /// - caches textures so the same file is only loaded once
    /// - owns and disposes of all cached textures
    /// - creates solid-color textures on demand
    /// - provides a reusable single-pixel white texture
    /// </para>
    /// </summary>
    internal sealed class MonoGameFileStreamTextureFactory : ITextureFactory, IDisposable
    {
        /// <summary>
        /// Graphics device required to allocate GPU textures.
        /// The factory does NOT own this and must NOT dispose it.
        /// Ill kill you if you dispose >;3
        /// </summary>
        private readonly GraphicsDevice graphicsDevice;


        private readonly Dictionary<string, ITexture> textureCache = new();

        private bool isDisposed;

        /// <summary>
        /// A single 1x1 white pixel texture.
        /// Useful for drawing rectangles, lines, and debug shapes.
        /// Owned and disposed by this factory.
        /// </summary>
        public ITexture SinglePixelTexture { get; }

        /// <summary>
        /// Creates the texture factory and immediately constructs the 1x1 white pixel texture.
        /// </summary>
        public MonoGameFileStreamTextureFactory(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            Texture2D pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            pixelTexture.SetData([XnaColor.White]);
            SinglePixelTexture = new MonoGameTexture(pixelTexture);
        }

        /// <summary>
        /// Creates a texture completely filled with a single color.
        /// The caller owns the returned texture and is responsible for disposing it.
        /// These textures are NOT cached.
        /// </summary>
        public ITexture CreateSolidColor(int width, int height, FletchColor color)
        {
            Texture2D texture = new Texture2D(graphicsDevice, width, height);

            XnaColor xnaColor = new XnaColor(color.R, color.G, color.B, color.A);

            XnaColor[] data = new XnaColor[width * height];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = xnaColor;
            }

            texture.SetData(data);

            return new MonoGameTexture(texture);
        }

        /// <summary>
        /// Loads a texture from a file path. Supports any format that Texture2D.FromStream does.
        /// Loaded textures are cached and owned by the factory.
        /// </summary>
        public ITexture Load(string path)
        {
            if (textureCache.TryGetValue(path, out ITexture? cachedTexture))
                return cachedTexture;

            Texture2D texture = LoadTextureFromFile(path);

            ITexture fletchTexture = new MonoGameTexture(texture);
            textureCache[path] = fletchTexture;

            return fletchTexture;
        }

        // Loads a Texture2D from a file using a FileStream, will have other ways to load in future.
        private Texture2D LoadTextureFromFile(string path)
        {
            using FileStream stream = File.OpenRead(path);
            return Texture2D.FromStream(graphicsDevice, stream);
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            foreach (ITexture texture in textureCache.Values)
            {
                (texture as IDisposable)?.Dispose();
            }

            textureCache.Clear();

            (SinglePixelTexture as IDisposable)?.Dispose();

            isDisposed = true;
        }
    }
}
