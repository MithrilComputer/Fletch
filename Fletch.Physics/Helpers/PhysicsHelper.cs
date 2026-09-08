using Fletch.Physics.Components;
using Fletch.Physics.Model.Info.IO;

namespace Fletch.Physics.Helpers
{
    internal static class PhysicsHelper
    {
        internal static BodyWriteState CreateBodyWriteState(RigidBody rigidBody)
        {
            return new BodyWriteState(
                rigidBody.CurrentPose.Position,
                rigidBody.CurrentPose.Rotation,
                rigidBody.LinearVelocity,
                rigidBody.AngularVelocity,
                rigidBody.GravityScale,
                rigidBody.LinearDrag,
                rigidBody.AngularDrag,
                rigidBody.FixedRotation,
                rigidBody.LockX,
                rigidBody.LockY,
                rigidBody.IsEnabled);
        }

        internal static void ApplyReadStateToRigidBody(RigidBody rigidBody, BodyReadState readState)
        {
            rigidBody.CurrentPose.Position = readState.Position;
            rigidBody.CurrentPose.Rotation = readState.Rotation;
            rigidBody.LinearVelocity = readState.LinearVelocity;
            rigidBody.AngularVelocity = readState.AngularVelocity;
        }
    }
}
