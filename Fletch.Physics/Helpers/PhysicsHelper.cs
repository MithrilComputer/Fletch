using Fletch.Physics.Abstractions.CollisionShapes;
using Fletch.Physics.Components;
using Fletch.Physics.Components.CollisionShapes;
using Fletch.Physics.Model.Info.IO;
using Fletch.Physics.Model.Info.Masking.Collisions;
using Fletch.Physics.Model.Info.Shapes;
using System.Numerics;

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

        internal static ColliderWriteState CreateColliderWriteState(Collider colliderType)
        {
            switch(colliderType)
            {
                case BoxCollider boxCollider:
                    return new ColliderWriteState(
                        boxCollider.Material,
                        new RectangleData(
                            new Vector2(
                                boxCollider.Width,
                                boxCollider.Height)),
                        new CollisionFilter(
                            CollisionCategory.All,
                            CollisionCategory.All),
                        colliderType.Offset,
                        0f,
                        false);

                case CircleCollider circleCollider:
                    return new ColliderWriteState(
                        circleCollider.Material,
                        new CircleData(
                            circleCollider.Radius),
                        new CollisionFilter(
                            CollisionCategory.All,
                            CollisionCategory.All),
                        colliderType.Offset,
                        0f,
                        false);

                case CapsuleCollider capsuleCollider:
                    return new ColliderWriteState(
                        capsuleCollider.Material,
                        new CapsuleData(
                            capsuleCollider.Radius,
                            capsuleCollider.Height),
                        new CollisionFilter(
                            CollisionCategory.All,
                            CollisionCategory.All),
                        colliderType.Offset,
                        0f,
                        false);

                default:
                    throw new ArgumentException($"Unsupported collider type: {colliderType.GetType().Name}");
            }
        }
    }
}
