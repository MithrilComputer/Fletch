using System.Numerics;

namespace Fletch.Physics.Model.Info.IO
{
    internal class BodyWriteState
    {
        public readonly Vector2 Position;
        public readonly float Rotation;

        public readonly Vector2 LinearVelocity;
        public readonly float AngularVelocity;

        public readonly float GravityScale;
        public readonly float LinearDamping;
        public readonly float AngularDamping;

        public readonly bool FixedRotation;
        public readonly bool Enabled;

        public BodyWriteState(
            Vector2 position,
            float rotation,
            Vector2 linearVelocity,
            float angularVelocity,
            float gravityScale,
            float linearDamping,
            float angularDamping,
            bool fixedRotation,
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
            Enabled = enabled;
        }
    }
}
