using Fletch.Physics.Model.Info.Collisions;
using Fletch.Physics.Model.Info.IO;
using Fletch.Physics.Model.ResourceHandles;
using System.Numerics;

namespace Fletch.Physics.Abstractions.Backends
{
    internal interface IPhysicsBackend
    {
        // running ---------------------

        void Initialize();

        void StepWorld(IWorldHandle world, float deltaTime);

        // creation and destruction ----
        IWorldHandle CreateWorld(Vector2 gravity);

        IBodyHandle CreateBody(IWorldHandle worldHandle, BodyWriteState writeState);

        IColliderHandle CreateCollider(IBodyHandle body, ColliderWriteState writeState);


        void DestroyWorld(IWorldHandle world);

        void DestroyBody(IBodyHandle body);

        void DestroyCollider(IColliderHandle collider);


        // state management ------------

        void SetBodyState(IBodyHandle bodyHandle, in BodyWriteState writeState);

        BodyReadState GetBodyState(IBodyHandle bodyHandle);


        void SetColliderState(IColliderHandle colliderHandle, ColliderWriteState writeState);


        // collision events -------------

        IReadOnlyCollection<BackendCollisionEvent> GetCollisionEvents(IWorldHandle world);


        // forces and impulses -----------

        void ApplyForce(IBodyHandle bodyHandle, Vector2 force);

        void ApplyForceAt(IBodyHandle bodyHandle, Vector2 force, Vector2 point);

        void ApplyImpulse(IBodyHandle bodyHandle, Vector2 impulse);

        void ApplyImpulseAt(IBodyHandle bodyHandle, Vector2 impulse, Vector2 point);

        void ApplyTorque(IBodyHandle bodyHandle, float torque);

        void ApplyAngularImpulse(IBodyHandle bodyHandle, float angularImpulse);
    }
}
