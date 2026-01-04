using Fletch.Core.Math.Geometry;
using Fletch.Rendering.Abstractions.Cameras;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System.Numerics;
using SysMatrix = System.Numerics.Matrix3x2;
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
        public Vector2 GetCameraCenter()
        {
            return new Vector2(camera.Center.X, camera.Center.Y);
        }

        /// <summary>
        /// Gets the screen position corresponding to the given world position.
        /// </summary>
        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            XnaVector2 screenPosition = camera.WorldToScreen(
                new XnaVector2(worldPosition.X, worldPosition.Y));

            return new Vector2(screenPosition.X, screenPosition.Y);
        }

        /// <summary>
        /// Gets the world position corresponding to the given screen position.
        /// </summary>
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            XnaVector2 worldPosition = camera.ScreenToWorld(
                new XnaVector2(screenPosition.X, screenPosition.Y));

            return new Vector2(worldPosition.X, worldPosition.Y);
        }

        /// <summary>
        /// Returns true if the given point is within the camera's visible area.
        /// </summary>
        /// <remarks>In world coords</remarks>
        public bool ContainsPoint(Vector2 point)
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
            var xna = camera.GetViewMatrix();

            return new SysMatrix(
                xna.M11, xna.M12,
                xna.M21, xna.M22,
                xna.M41, xna.M42);
        }

        /// <summary>
        /// Sets the position of the camera in world coordinates.
        /// </summary>
        public void SetPosition(Vector2 position)
        {
            camera.Position = new XnaVector2(position.X, position.Y);
        }

        /// <summary>
        /// Moves the camera by the given delta in world coordinates.
        /// </summary>
        public void MoveBy(Vector2 delta)
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
        public Vector2 GetPosition() =>
            new Vector2(camera.Position.X, camera.Position.Y);

        /// <summary>
        /// Gets the zoom level of the camera.
        /// </summary>
        public float GetZoom() => camera.Zoom;
    }
}
