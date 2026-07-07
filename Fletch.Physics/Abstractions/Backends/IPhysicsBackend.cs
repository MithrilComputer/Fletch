using Fletch.Physics.Model;
using Fletch.Physics.Model.Info.Collisions;
using Fletch.Physics.Model.Info.IO;
using Fletch.Physics.Model.Info.Shapes;
using Fletch.Physics.Model.ResourceHandles;
using System.Numerics;

namespace Fletch.Physics.Abstractions.Backends
{
    internal interface IPhysicsBackend
    {
        void Initalize();

        void StepWorld(IWorldHandle world, float deltaTime);

        
        IWorldHandle CreateWorld(Vector2 gravity);

        IBodyHandle CreateBody(Vector2 Position, float rotation, PhysicsMode physicsMode);

        IColliderHandle CreateCollider(IBodyHandle body, PhysicsMaterial physicsMaterial, ShapeData shapeData);


        void DestroyWorld(IWorldHandle world);

        void DestroyBody(IBodyHandle body);

        void DestroyCollider(IColliderHandle body);


        void SetBodyState(IBodyHandle bodyHandle, in BodyWriteState writeState);

        BodyReadState GetBodyState(IBodyHandle bodyHandle);


        void SetColiderState(IColliderHandle colliderHandle, ColliderWriteState writeState);


        IReadOnlyCollection<CollisionEventInfo> GetCollisionEvents(IWorldHandle world);
    }
}
