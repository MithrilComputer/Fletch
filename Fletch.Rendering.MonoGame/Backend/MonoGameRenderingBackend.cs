using Fletch.Core.EngineConfig;
using Fletch.Core.Math.Geometry;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Rendering.Abstractions.Cameras;
using Fletch.Rendering.Abstractions.Drawing;
using Fletch.Rendering.Abstractions.Factories;
using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.Model;
using Fletch.Rendering.MonoGame.Cameras;
using Fletch.Rendering.MonoGame.Drawing;
using Fletch.Rendering.MonoGame.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;
using FletchColor = Fletch.Core.Colors.Color;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaViewport = Microsoft.Xna.Framework.Graphics.Viewport;

namespace Fletch.Rendering.MonoGame.Backend
{
    /// <summary>
    /// MonoGame-backed implementation of <see cref="IRenderingBackend"/>.
    /// Manages cameras, sprite batching, frame lifecycle, and render resources.
    /// </summary>
    internal sealed class MonoGameRenderingBackend : IRenderingBackend
    {
        private bool isInitialized = false;

        private MonoGameDebugRenderer debugRenderer;

        private MonoGameSpriteBatcher spriteBatcher;

        private MonoGameCamera mainCamera;

        private List<ICamera> cameras = new List<ICamera>();

        private MonoGameFontFactory fontFactory;

        private MonoGameRenderTargetFactory renderTargetFactory;

        private MonoGameFileStreamTextureFactory textureFactory;

        private GameWindow gameWindow;

        private GraphicsDevice graphicsDevice;

        private ICamera? currentCamera;

        private RectangleInt fullViewport;

        private BoxingViewportAdapter viewportAdapter;

        /// <summary>
        /// Gets the primary <see cref="ISpriteBatcher"/> used for batched 2D rendering.
        /// </summary>
        public ISpriteBatcher SpriteBatcher { get { return spriteBatcher; } }

        /// <summary>
        /// Gets the factory used to create and cache font resources.
        /// </summary>
        public IFontFactory FontFactory { get { return fontFactory; } }

        /// <summary>
        /// Gets the MonoGame Texture Factory.
        /// </summary>
        public ITextureFactory TextureFactory { get { return textureFactory; } }

        /// <summary>
        /// Gets the debug renderer used for shapes, lines, and diagnostic overlays.
        /// </summary>
        public IDebugRenderer DebugRenderer { get { return debugRenderer; } }

        /// <summary>
        /// Gets the main camera. This camera always exists and cannot be removed.
        /// </summary>
        public ICamera MainCamera { get { return mainCamera; } }

        /// <summary>
        /// Gets a read-only list of all cameras registered with this backend.
        /// </summary>
        public IReadOnlyList<ICamera> Cameras { get { return cameras; } }

        /// <summary>
        /// Initializes the rendering backend, Required to run before it can be used.
        /// </summary>
        public void Initialize(Game game)
        {
            graphicsDevice = game.GraphicsDevice;

            gameWindow = game.Window;

            fullViewport = new RectangleInt(
                0,
                0,
                graphicsDevice.PresentationParameters.BackBufferWidth,
                graphicsDevice.PresentationParameters.BackBufferHeight);

            viewportAdapter = new BoxingViewportAdapter(
                gameWindow,
                graphicsDevice,
                EngineConfig.VirtualResolution.X,
                EngineConfig.VirtualResolution.Y
                );

            mainCamera = new MonoGameCamera(viewportAdapter);

            cameras.Add(mainCamera);

            spriteBatcher = new MonoGameSpriteBatcher(graphicsDevice);

            textureFactory = new MonoGameFileStreamTextureFactory(graphicsDevice);

            fontFactory = new MonoGameFontFactory();

            renderTargetFactory = new MonoGameRenderTargetFactory(graphicsDevice);

            debugRenderer = new MonoGameDebugRenderer(spriteBatcher, textureFactory.SinglePixelTexture);

            gameWindow.ClientSizeChanged += OnResize;

            isInitialized = true;
        }

        /// <summary>
        /// Begins rendering using the specified camera and viewport.
        /// Must be followed by a call to <see cref="EndCamera"/>.
        /// </summary>
        /// <param name="camera">Camera whose view matrix will be applied.</param>
        /// <param name="virtualViewport">Viewport rectangle in back-buffer coordinates.</param>
        /// <param name="blendMode">Blend mode to use for sprite rendering.</param>
        /// <param name="samplerMode">Texture sampling mode.</param>
        /// <exception cref="InvalidOperationException">Thrown if a camera is already active.</exception>
        public void BeginCamera(ICamera camera, RectangleInt virtualViewport, BlendMode blendMode = BlendMode.Alpha, SamplerMode samplerMode = SamplerMode.Linear)
        {
            if (!isInitialized)
                throw new InvalidOperationException("Backend Not Initialized Yet!");

            if (currentCamera != null)
                throw new InvalidOperationException("Camera already active.");

            if (!cameras.Contains(camera))
                throw new InvalidOperationException("Camera is not registered with this backend.");

            currentCamera = camera;

            // Covert virtual viewport to real viewport
            XnaViewport realViewport = VirtualViewportToRealViewport(virtualViewport);

            graphicsDevice.Viewport = new XnaViewport(realViewport.X, realViewport.Y, realViewport.Width, realViewport.Height);

            spriteBatcher.Begin(currentCamera.GetViewMatrix(), blendMode, samplerMode);
        }

        /// <summary>
        /// Ends rendering for the currently active camera.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if no camera is active.</exception>
        public void EndCamera()
        {
            if (!isInitialized)
                throw new InvalidOperationException("Backend Not Initialized Yet!");

            if (currentCamera == null)
                throw new InvalidOperationException("No active camera.");

            spriteBatcher.End();

            currentCamera = null;
        }

        /// <summary>
        /// Begins a new frame by clearing the back buffer to the given color.
        /// </summary>
        /// <param name="clearColor">The color used to clear the framebuffer.</param>
        /// <exception cref="InvalidOperationException">Thrown if a camera is currently active.</exception>
        public void BeginFrame(FletchColor clearColor)
        {
            if (!isInitialized)
                throw new InvalidOperationException("Backend Not Initialized Yet!");

            if (currentCamera != null)
                throw new InvalidOperationException("Cannot begin frame while a camera is active.");

            graphicsDevice.SetRenderTarget(null);

            graphicsDevice.Viewport = new Viewport(
                0,
                0,
                fullViewport.Width,
                fullViewport.Height);

            XnaColor xnaColor = new XnaColor(
                clearColor.R,
                clearColor.G,
                clearColor.B,
                clearColor.A);

            graphicsDevice.Clear(xnaColor);
        }

        /// <summary>
        /// Ends the current frame.
        /// </summary>
        /// <remarks>
        /// Currently performs no work, but exists for API symmetry and future expansion.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if a camera is currently active.</exception>
        public void EndFrame()
        {
            if (!isInitialized)
                throw new InvalidOperationException("Backend Not Initialized Yet!");

            if (currentCamera != null)
                throw new InvalidOperationException("Cannot end frame while a camera is active.");

            // Not much to do yet, how peaceful
        }

        /// <summary>
        /// Creates a new camera managed by this backend.
        /// </summary>
        /// <returns>The newly created camera.</returns>
        public ICamera CreateCamera()
        {
            if (!isInitialized)
                throw new InvalidOperationException("Backend Not Initialized Yet!");

            MonoGameCamera camera = new MonoGameCamera(viewportAdapter);

            cameras.Add(camera);

            return camera;
        }

        /// <summary>
        /// Removes a camera from the backend and disposes it if applicable.
        /// </summary>
        /// <param name="camera">The camera to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown when camera is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if removing the main camera, an active camera, or a camera not registered.
        /// </exception>
        public void RemoveCamera(ICamera camera)
        {
            if (!isInitialized)
                throw new InvalidOperationException("Backend Not Initialized Yet!");

            ArgumentNullException.ThrowIfNull(camera);

            if (ReferenceEquals(camera, MainCamera))
                throw new InvalidOperationException("Cannot remove the main camera.");

            if (ReferenceEquals(camera, currentCamera))
                throw new InvalidOperationException("Cannot remove a camera while it is active.");

            if (!cameras.Remove(camera))
                throw new InvalidOperationException("Camera is not registered with this backend.");

            if (camera is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// Creates a new render target with the specified pixel dimensions.
        /// </summary>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        /// <returns>A new rendering target.</returns>
        public IRenderingTarget CreateRenderTarget(int width, int height)
        {
            if (!isInitialized)
                throw new InvalidOperationException("Backend Not Initialized Yet!");

            return renderTargetFactory.Create(width, height);
        }

        /// <summary>
        /// Converts a camera's viewport defined in VIRTUAL coordinates (inside the virtual resolution)
        /// into a REAL GraphicsDevice Viewport (inside the backbuffer), respecting BoxingViewportAdapter.
        /// </summary>
        private Viewport VirtualViewportToRealViewport(RectangleInt virtualViewportRect)
        {
            // This is the real boxed area (already centered with black bars)
            Viewport boxedReal = graphicsDevice.Viewport;

            // Uniform scale used by boxing
            float scaleX = boxedReal.Width / (float)viewportAdapter.VirtualWidth;
            float scaleY = boxedReal.Height / (float)viewportAdapter.VirtualHeight;

            // In boxing these should match; use X (or Min to be safe)
            float scale = MathF.Min(scaleX, scaleY);

            int realX = boxedReal.X + (int)MathF.Round(virtualViewportRect.X * scale);
            int realY = boxedReal.Y + (int)MathF.Round(virtualViewportRect.Y * scale);

            int realW = (int)MathF.Round(virtualViewportRect.Width * scale);
            int realH = (int)MathF.Round(virtualViewportRect.Height * scale);

            // Clamp so you never spill outside the boxed area (nice for edge rounding)
            int maxX = boxedReal.X + boxedReal.Width;
            int maxY = boxedReal.Y + boxedReal.Height;

            if (realX < boxedReal.X) realX = boxedReal.X;
            if (realY < boxedReal.Y) realY = boxedReal.Y;

            if (realX + realW > maxX) realW = maxX - realX;
            if (realY + realH > maxY) realH = maxY - realY;

            return new Viewport(realX, realY, realW, realH);
        }

        private void OnResize(object? sender, EventArgs e)
        {
            if (!isInitialized)
                throw new InvalidOperationException("Backend Not Initialized Yet!");

            XnaViewport viewport = graphicsDevice.Viewport;

            fullViewport = new RectangleInt(viewport.X, viewport.Y, viewport.Width, viewport.Height);
        }

        public void Dispose()
        {
            gameWindow.ClientSizeChanged -= OnResize;

            spriteBatcher.Dispose();
            textureFactory.Dispose();
            fontFactory.Dispose();

            viewportAdapter.Dispose();

            currentCamera = null;

            foreach (var camera in cameras)
            {
                if (camera is IDisposable disposable)
                    disposable.Dispose();
            }

            cameras.Clear();
        }
    }
}
