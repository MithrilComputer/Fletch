using Fletch.Core.Math.Geometry;
using Fletch.Rendering.Abstractions.Cameras;
using Fletch.Rendering.Abstractions.Drawing;
using Fletch.Rendering.Abstractions.Factories;
using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.Model;

namespace Fletch.Rendering.Abstractions.Backends
{
    internal interface IRenderingBackend : IDisposable
    {
        /// <summary>
        /// Begins a new frame by clearing the back buffer to the given color.
        /// </summary>
        /// <param name="clearColor">The color used to clear the framebuffer.</param>
        /// <exception cref="InvalidOperationException">Thrown if a camera is currently active.</exception>
        void BeginFrame(Color clearColor);

        /// <summary>
        /// Ends the current frame.
        /// </summary>
        /// <remarks>
        /// Currently performs no work, but exists for API symmetry and future expansion.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if a camera is currently active.</exception>
        void EndFrame();

        /// <summary>
        /// Gets the primary <see cref="ISpriteBatcher"/> used for batched 2D rendering.
        /// </summary>
        ISpriteBatcher SpriteBatcher { get; }

        /// <summary>
        /// Gets the debug renderer used for shapes, lines, and diagnostic overlays.
        /// </summary>
        IDebugRenderer DebugRenderer { get; }

        /// <summary>
        /// Gets the main camera. This camera always exists and cannot be removed.
        /// </summary>
        ICamera MainCamera { get; }

        /// <summary>
        /// Gets a read-only list of all cameras registered with this backend.
        /// </summary>
        IReadOnlyList<ICamera> Cameras { get; }

        /// <summary>
        /// Gets the factory used to create and cache font resources.
        /// </summary>
        IFontFactory FontFactory { get; }

        /// <summary>
        /// Gets the texture factory for the backend.
        /// </summary>
        ITextureFactory TextureFactory { get; }

        /// <summary>
        /// Creates a new camera managed by this backend.
        /// </summary>
        /// <returns>The newly created camera.</returns>
        ICamera CreateCamera();

        /// <summary>
        /// Removes a camera from the backend and disposes it if applicable.
        /// </summary>
        /// <param name="camera">The camera to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown when camera is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if removing the main camera, an active camera, or a camera not registered.
        /// </exception>
        void RemoveCamera(ICamera camera);

        /// <summary>
        /// Begins rendering using the specified camera and viewport.
        /// Must be followed by a call to <see cref="EndCamera"/>.
        /// </summary>
        /// <param name="camera">Camera whose view matrix will be applied.</param>
        /// <param name="viewport">Viewport rectangle in back-buffer coordinates.</param>
        /// <param name="blendMode">Blend mode to use for sprite rendering.</param>
        /// <param name="samplerMode">Texture sampling mode.</param>
        /// <exception cref="InvalidOperationException">Thrown if a camera is already active.</exception>
        void BeginCamera(
            ICamera camera,
            RectangleInt viewport,
            BlendMode blendMode = BlendMode.Alpha,
            SamplerMode samplerMode = SamplerMode.Linear);

        /// <summary>
        /// Ends rendering for the currently active camera.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if no camera is active.</exception>
        void EndCamera();


        //TODO For later reminder of post processing
        IRenderingTarget CreateRenderTarget(int width, int height);
    }
}
