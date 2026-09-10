using Fletch.Physics.Abstractions.Backends;
using Fletch.Physics.Abstractions.CollisionShapes;
using Fletch.Physics.Components.CollisionShapes;
using Fletch.Physics.Model.Info.IO;
using Fletch.Physics.Model.Info.Shapes;
using Fletch.Physics.Model.ResourceHandles;

namespace Fletch.Physics.Factories
{
    internal class ColliderFactory
    {
        private readonly IPhysicsBackend physicsBackend;

        public ColliderFactory(IPhysicsBackend physicsBackend) 
        { 
            this.physicsBackend = physicsBackend;
        
        
        }

        public Collider CreateNewCollider<>(IBodyHandle bodyHandle, ColliderWriteState writeState)
        {
            switch (writeState.Shape)
            {
                case RectangleData boxShape:

                    if (writeState.Shape is not RectangleData boxShape)
                        throw new ArgumentException("Invalid shape data for BoxCollider");

                    IColliderHandle colliderHandle = physicsBackend.CreateCollider(bodyHandle, writeState);

                    BoxCollider boxCollider = new BoxCollider(
                        boxShape.Size.X, 
                        boxShape.Size.Y, 
                        bodyHandle, 
                        colliderHandle, 
                        writeState.Material,
                        writeState.Offset);

                    break;

                case Type t when t == typeof(CircleCollider):
                    break;

                case Type t when t == typeof(CapsuleCollider):
                    break;

                default:
                    throw new ArgumentException("Unsupported collider type of " + typeof(T).Name);
            }
        }
    }
}
