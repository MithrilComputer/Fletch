using Fletch.Core.Math.Geometry;
using Fletch.Rendering.Abstractions.Cameras;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System.Numerics;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using XnaVector2 = Microsoft.Xna.Framework.Vector2;
using SysMaxtrix = Microsoft.Xna.Framework.Matrix;

namespace Fletch.Rendering.MonoGame.Cameras
{
    internal class MonoGameCamera : ICamera
    {
        private readonly OrthographicCamera camera;
        private readonly BoxingViewportAdapter viewportAdapter;

        public MonoGameCamera(BoxingViewportAdapter viewportAdapter)
        {
            this.viewportAdapter = viewportAdapter;
            camera = new OrthographicCamera(viewportAdapter);
        }

        public RectangleFloat GetVisibleArea()
        {
            RectangleF bounds = camera.BoundingRectangle;

            return new RectangleFloat(bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }

        public Vector2 GetCameraCenter()
        {
            return new Vector2(camera.Center.X, camera.Center.Y);
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            XnaVector2 screenPosition = camera.WorldToScreen(
                new XnaVector2(worldPosition.X, worldPosition.Y));

            return new Vector2(screenPosition.X, screenPosition.Y);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            XnaVector2 worldPosition = camera.ScreenToWorld(
                new XnaVector2(screenPosition.X, screenPosition.Y));

            return new Vector2(worldPosition.X, worldPosition.Y);
        }

        public bool ContainsPoint(Vector2 point)
        {
            XnaVector2 xnaPoint = new XnaVector2(point.X, point.Y);

            return camera.BoundingRectangle.Contains(xnaPoint);
        }

        public void SetWorldBounds(RectangleFloat bounds)
        {
            float viewWorldWidth = viewportAdapter.VirtualWidth / camera.Zoom;
            float viewWorldHeight = viewportAdapter.VirtualHeight / camera.Zoom;

            float diagonal = MathF.Sqrt(viewWorldWidth * viewWorldWidth +
                                    viewWorldHeight * viewWorldHeight);

            float padX = (diagonal - viewWorldWidth) * 0.5f;
            float padY = (diagonal - viewWorldHeight) * 0.5f;

            float paddedX = bounds.X - padX;
            float paddedY = bounds.Y - padY;
            float paddedWidth = bounds.Width + padX * 2f;
            float paddedHeight = bounds.Height + padY * 2f;

            XnaRectangle xnaBounds = new XnaRectangle(
                (int)paddedX,
                (int)paddedY,
                (int)paddedWidth,
                (int)paddedHeight);

            camera.EnableWorldBounds(xnaBounds);
        }

        SysMaxtrix GetViewMatrix()
        {
            var matrix = camera.GetViewMatrix();

            return new SysMaxtrix(
                matrix.M11, matrix.M12,
                matrix.M21, matrix.M22,
                matrix.M41, matrix.M42);
        }

        public void SetPosition(Vector2 position)
        {
            camera.Position = new XnaVector2(position.X, position.Y);
        }

        public void SetZoom(float zoom)
        {
            camera.Zoom = zoom;
        }

        public void SetRotation(float rotation)
        {
            camera.Rotation = rotation;
        }
    }
}
