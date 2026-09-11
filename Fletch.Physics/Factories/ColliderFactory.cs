using Fletch.Physics.Abstractions.Backends;
using Fletch.Physics.Abstractions.CollisionShapes;
using Fletch.Physics.Components;
using Fletch.Physics.Components.CollisionShapes;
using Fletch.Physics.Maps;
using Fletch.Physics.Model.Info.IO;
using Fletch.Physics.Model.Info.Shapes;
using Fletch.Physics.Model.ResourceHandles;

namespace Fletch.Physics.Factories
{
    internal class ColliderFactory
    {
        private readonly IPhysicsBackend physicsBackend;

        private readonly ColliderRigidbodyMap colliderRigidbodyMap;

        public ColliderFactory(IPhysicsBackend physicsBackend, ColliderRigidbodyMap colliderRigidbodyMap)
        {
            this.physicsBackend = physicsBackend;
            this.colliderRigidbodyMap = colliderRigidbodyMap;
        }

        public Collider CreateNewCollider(RigidBody parentBody, ColliderWriteState writeState)
        {
            if (!parentBody.Initialized)
                throw new InvalidOperationException("Parent body is not Initalized, can not add new Collider");

            switch (writeState.Shape)
            {
                case RectangleData boxShape:

                    IColliderHandle boxColliderHandle = physicsBackend.CreateCollider(parentBody.BodyHandle, writeState);

                    colliderRigidbodyMap.AddMapping(boxColliderHandle, parentBody);

                    return new BoxCollider(
                        boxShape.Size.X,
                        boxShape.Size.Y,
                        parentBody.BodyHandle,
                        boxColliderHandle,
                        writeState.Material,
                        writeState.Offset);

                case CircleData circleShape:

                    IColliderHandle circleColliderHandle = physicsBackend.CreateCollider(parentBody.BodyHandle, writeState);

                    colliderRigidbodyMap.AddMapping(circleColliderHandle, parentBody);

                    return new CircleCollider(
                        circleShape.Radius,
                        parentBody.BodyHandle,
                        circleColliderHandle,
                        writeState.Material,
                        writeState.Offset);

                case CapsuleData capsuleShape:

                    IColliderHandle capsoleColliderHandle = physicsBackend.CreateCollider(parentBody.BodyHandle, writeState);

                    colliderRigidbodyMap.AddMapping(capsoleColliderHandle, parentBody);

                    return new CapsuleCollider(
                        capsuleShape.Radius,
                        capsuleShape.Height,
                        parentBody.BodyHandle,
                        capsoleColliderHandle,
                        writeState.Material,
                        writeState.Offset);

                default:
                    throw new ArgumentException("Unsupported shapedata type of " + writeState.Shape.GetType().Name);
            }
        }
    }
}
