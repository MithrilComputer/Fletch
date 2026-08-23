using System.Numerics;

namespace Fletch.Rendering.Model
{
    /// <summary>
    /// A class that represents the position and rotation of a renderable object in 2D space.
    /// </summary>
    public class RenderPose
    {
        /// <summary>
        /// position of the renderable object in 2D space.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// The rotation of the renderable object in radians.
        /// </summary>
        public float Rotation { get; set; }

        public RenderPose(Vector2 position, float rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
