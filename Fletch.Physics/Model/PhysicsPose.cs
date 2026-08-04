using System.Numerics;

namespace Fletch.Physics.Model
{
    /// <summary>
    /// A class that represents the position and rotation of a physics object in 2D space.
    /// </summary>
    internal class PhysicsPose
    {
        /// <summary>
        /// position of the physics object in 2D space.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// The rotation of the physics object in radians.
        /// </summary>
        public float Rotation { get; set; }

        public PhysicsPose(Vector2 position, float rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
