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
        void Initialize();

        void StepWorld(IWorldHandle world, float deltaTime);

        
        IWorldHandle CreateWorld(Vector2 gravity);

        IBodyHandle CreateBody(IWorldHandle worldHandle, BodyWriteState writeState);

        IColliderHandle CreateCollider(IBodyHandle body, ColliderWriteState writeState);


        void DestroyWorld(IWorldHandle world);

        void DestroyBody(IBodyHandle body);

        void DestroyCollider(IColliderHandle collider);


        void SetBodyState(IBodyHandle bodyHandle, in BodyWriteState writeState);

        BodyReadState GetBodyState(IBodyHandle bodyHandle);


        void SetColliderState(IColliderHandle colliderHandle, ColliderWriteState writeState);


        IReadOnlyCollection<CollisionEventInfo> GetCollisionEvents(IWorldHandle world);
    }
}
