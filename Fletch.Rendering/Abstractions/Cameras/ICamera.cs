using Fletch.Core.Math.Geometry;
using System.Numerics;

namespace Fletch.Rendering.Abstractions.Cameras
{
    /// <summary>
    /// Represents a 2D camera for rendering scenes.
    /// </summary>
    internal interface ICamera
    {
        /// <summary>
        /// Gets the area currently visible by the camera in world coordinates.
        /// </summary>
        RectangleFloat GetVisibleArea();

        /// <summary>
        /// Forces the viewport to recompute boxing and scaling.
        /// Useful after window size, backbuffer, or virtual resolution changes.
        /// </summary>
        void ResetViewport();

        /// <summary>
        /// Gets the center point of the camera in world coordinates.
        /// </summary>
        Vector2 GetCameraCenter();

        /// <summary>
        /// Converts a world position to a screen position.
        /// </summary>
        Vector2 WorldToScreen(Vector2 worldPosition);

        /// <summary>
        /// Converts a screen position to a world position.
        /// </summary>
        Vector2 ScreenToWorld(Vector2 screenPosition);

        /// <summary>
        /// Returns true if the given world-space point is within the camera's visible area.
        /// </summary>
        bool ContainsPoint(Vector2 point);

        /// <summary>
        /// Sets the world bounds for the camera in world coordinates.
        /// </summary>
        void SetWorldBounds(RectangleFloat bounds);

        /// <summary>
        /// Gets the view matrix of the camera.
        /// </summary>
        Matrix3x2 GetViewMatrix();

        /// <summary>
        /// Sets the position of the camera in world coordinates.
        /// </summary>
        void SetPosition(Vector2 position);

        /// <summary>
        /// Moves the camera by the given delta in world coordinates.
        /// </summary>
        void MoveBy(Vector2 delta);

        /// <summary>
        /// Sets the zoom level of the camera.
        /// </summary>
        void SetZoom(float zoom);

        /// <summary>
        /// Gets the position of the camera in world coordinates.
        /// </summary>
        Vector2 GetPosition();

        /// <summary>
        /// Gets the zoom level of the camera.
        /// </summary>
        float GetZoom();
    }
}
