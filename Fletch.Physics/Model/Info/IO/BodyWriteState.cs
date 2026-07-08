using System.Numerics;

namespace Fletch.Physics.Model.Info.IO
{
    internal class BodyWriteState
    {
        public Vector2 Position { get; }
        public float Rotation { get; }

        public PhysicsMode Mode { get; }

        public Vector2 LinearVelocity { get; }
        public float AngularVelocity { get; }

        public float GravityScale { get; }
        public float LinearDamping { get; }
        public float AngularDamping { get; }

        public bool FixedRotation { get; }
        public bool LockX { get; }
        public bool LockY { get; }
        public bool Enabled { get; }
        public bool UseInterpolation { get; }

        public BodyWriteState(
            Vector2 position,
            float rotation,
            Vector2 linearVelocity,
            float angularVelocity,
            float gravityScale,
            float linearDamping,
            float angularDamping,
            bool fixedRotation,
            bool lockX,
            bool lockY,
            bool enabled)
        {
            Position = position;
            Rotation = rotation;
            LinearVelocity = linearVelocity;
            AngularVelocity = angularVelocity;
            GravityScale = gravityScale;
            LinearDamping = linearDamping;
            AngularDamping = angularDamping;
            FixedRotation = fixedRotation;
            LockX = lockX;
            LockY = lockY;
            Enabled = enabled;
        }
    }
}
