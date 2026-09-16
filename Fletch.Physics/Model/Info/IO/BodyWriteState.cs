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

        public float Mass { get; }

        public bool FixedRotation { get; }
        public bool LockX { get; }
        public bool LockY { get; }
        public bool Enabled { get; }
        public bool UseInterpolation { get; }

        public BodyWriteState(
            Vector2 position,
            float rotation,
            PhysicsMode mode,
            Vector2 linearVelocity,
            float angularVelocity,
            float gravityScale,
            float linearDamping,
            float angularDamping,
            bool fixedRotation,
            float mass,
            bool lockX,
            bool lockY,
            bool enabled,
            bool useInterpolation)
        {
            Position = position;
            Rotation = rotation;
            Mode = mode;
            LinearVelocity = linearVelocity;
            AngularVelocity = angularVelocity;
            GravityScale = gravityScale;
            LinearDamping = linearDamping;
            AngularDamping = angularDamping;
            FixedRotation = fixedRotation;
            Mass = mass;
            LockX = lockX;
            LockY = lockY;
            Enabled = enabled;
            UseInterpolation = useInterpolation;
        }
    }
}
