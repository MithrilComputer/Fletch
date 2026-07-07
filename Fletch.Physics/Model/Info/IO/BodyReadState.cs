using System.Numerics;

namespace Fletch.Physics.Model.Info.IO
{
    internal struct BodyReadState
    {
        public readonly Vector2 Position;
        public readonly float Rotation;

        public readonly Vector2 LinearVelocity;
        public readonly float AngularVelocity;

        public readonly bool IsAwake;

        public BodyReadState(
            Vector2 position,
            float rotation,
            Vector2 linearVelocity,
            float angularVelocity,
            bool isAwake)
        {
            Position = position;
            Rotation = rotation;
            LinearVelocity = linearVelocity;
            AngularVelocity = angularVelocity;
            IsAwake = isAwake;
        }
    }
}
