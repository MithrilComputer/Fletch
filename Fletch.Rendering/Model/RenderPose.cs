using System.Numerics;

namespace Fletch.Rendering.Model
{
    /// <summary>
    /// A class that represents the position and rotation of a renderable object in 2D space.
    /// </summary>
    public readonly struct RenderPose
    {
        /// <summary>
        /// position of the renderable object in 2D space.
        /// </summary>
        public Vector2 Position { get; }

        /// <summary>
        /// The rotation of the renderable object in radians.
        /// </summary>
        public float Rotation { get; }

        public RenderPose(Vector2 position, float rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
