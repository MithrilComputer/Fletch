using Fletch.Core.Math.Geometry;
using Fletch.Rendering.Abstractions.Cameras;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using SysMatrix = System.Numerics.Matrix3x2;
using SysVector = System.Numerics.Vector2;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using XnaVector2 = Microsoft.Xna.Framework.Vector2;

namespace Fletch.Rendering.MonoGame.Cameras
{
    internal class MonoGameCamera : ICamera
    {
        private readonly OrthographicCamera camera;
        private readonly BoxingViewportAdapter viewportAdapter;

        // Adjust these later, perhaps make configurable
        private readonly float zoomMax = 100.0f;
        private readonly float zoomMin = 0.05f;

        public MonoGameCamera(BoxingViewportAdapter viewportAdapter)
        {
            this.viewportAdapter = viewportAdapter;
            camera = new OrthographicCamera(viewportAdapter);
        }

        /// <summary>
        /// Gets the area currently visible by the camera in world coordinates.
        /// </summary>
        public RectangleFloat GetVisibleArea()
        {
            RectangleF bounds = camera.BoundingRectangle;

            return new RectangleFloat(bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }

        /// <summary>
        /// Forces the viewport adapter to recompute its boxing and scaling,
        /// useful after window size, backbuffer, or virtual resolution changes.
        /// </summary>
        public void ResetViewport()
        {
            viewportAdapter.Reset();
        }

        /// <summary>
        /// Gets the center point of the camera in world coordinates.
        /// </summary>
        public SysVector GetCameraCenter()
        {
            return new SysVector(camera.Center.X, camera.Center.Y);
        }

        /// <summary>
        /// Gets the screen position corresponding to the given world position.
        /// </summary>
        public SysVector WorldToScreen(SysVector worldPosition)
        {
            float height = viewportAdapter.VirtualHeight;

            XnaVector2 worldDown = new XnaVector2(worldPosition.X, height - worldPosition.Y);

            XnaVector2 screenPosition = camera.WorldToScreen(worldDown);

            return new SysVector(screenPosition.X, screenPosition.Y);
        }

        /// <summary>
        /// Gets the world position corresponding to the given screen position.
        /// </summary>
        public SysVector ScreenToWorld(SysVector screenPosition)
        {
            XnaVector2 worldDown = camera.ScreenToWorld(
                new XnaVector2(screenPosition.X, screenPosition.Y));

            float height = viewportAdapter.VirtualHeight;

            return new SysVector(worldDown.X, height - worldDown.Y);
        }

        /// <summary>
        /// Returns true if the given point is within the camera's visible area.
        /// </summary>
        /// <remarks>In world coords</remarks>
        public bool ContainsPoint(SysVector point)
        {
            XnaVector2 xnaPoint = new XnaVector2(point.X, point.Y);

            return camera.BoundingRectangle.Contains(xnaPoint);
        }

        /// <summary>
        /// Sets the world bounds for the camera in world coordinates.
        /// </summary>
        public void SetWorldBounds(RectangleFloat bounds)
        {
            XnaRectangle xnaBounds = new XnaRectangle(
                (int)bounds.X,
                (int)bounds.Y,
                (int)bounds.Width,
                (int)bounds.Height);

            camera.EnableWorldBounds(xnaBounds);
        }

        /// <summary>
        /// Gets the view matrix of the camera.
        /// </summary>
        public SysMatrix GetViewMatrix()
        {
            Matrix xna = camera.GetViewMatrix();

            SysMatrix veiw = new SysMatrix(
                xna.M11, xna.M12,
                xna.M21, xna.M22,
                xna.M41, xna.M42);

            float height = viewportAdapter.VirtualHeight;

            SysMatrix flipY = SysMatrix.CreateScale(1, -1) * SysMatrix.CreateTranslation(0, height);

            return veiw * flipY;
        }

        /// <summary>
        /// Sets the position of the camera in world coordinates.
        /// </summary>
        public void SetPosition(SysVector position)
        {
            camera.Position = new XnaVector2(position.X, position.Y);
        }

        /// <summary>
        /// Moves the camera by the given delta in world coordinates.
        /// </summary>
        public void MoveBy(SysVector delta)
        {
            camera.Position += new XnaVector2(delta.X, delta.Y);
        }

        /// <summary>
        /// Sets the zoom level of the camera.
        /// </summary>
        public void SetZoom(float zoom)
        {
            camera.Zoom = Math.Clamp(zoom, zoomMin, zoomMax);
        }
        
        /// <summary>
        /// Gets the position of the camera in world coordinates.
        /// </summary>
        public SysVector GetPosition() =>
            new SysVector(camera.Position.X, camera.Position.Y);

        /// <summary>
        /// Gets the zoom level of the camera.
        /// </summary>
        public float GetZoom() => camera.Zoom;
    }
}
