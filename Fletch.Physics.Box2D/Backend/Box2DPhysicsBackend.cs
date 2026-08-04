using Fletch.Physics.Abstractions.Backends;
using Fletch.Physics.Box2D.Natives;
using Fletch.Physics.Model.Info.Collisions;
using Fletch.Physics.Model.Info.IO;
using Fletch.Physics.Model.ResourceHandles;
using System.Numerics;

namespace Fletch.Physics.Box2D.Backend
{
    internal class Box2DPhysicsBackend : IPhysicsBackend
    {
        private readonly Box2DManager b2Manager;

        public void Initialize() { }

        public Box2DPhysicsBackend()
        {
            b2Manager = new Box2DManager();
        }

        public void StepWorld(IWorldHandle world, float deltaTime)
        {
            b2Manager.Step(world, deltaTime);
        }

        public IWorldHandle CreateWorld(Vector2 gravity)
        {
            return b2Manager.CreateWorld(gravity);
        }

        public IBodyHandle CreateBody(IWorldHandle worldHandle, BodyWriteState writeState)
        {
            return b2Manager.CreateBody(worldHandle, writeState);
        }

        public IColliderHandle CreateCollider(IBodyHandle body, ColliderWriteState writeState)
        {
            return b2Manager.CreateCollider(body, writeState);
        }


        public void DestroyWorld(IWorldHandle world)
        {
            b2Manager.DestroyResource(world);
        }

        public void DestroyBody(IBodyHandle body)
        {
            b2Manager.DestroyResource(body);
        }

        public void DestroyCollider(IColliderHandle collider)
        {
            b2Manager.DestroyResource(collider);
        }


        public void SetBodyState(IBodyHandle bodyHandle, in BodyWriteState writeState)
        {
            b2Manager.SetBodyState(bodyHandle, writeState);
        }

        public BodyReadState GetBodyState(IBodyHandle bodyHandle)
        {
            return b2Manager.GetBodyState(bodyHandle);
        }

        public void SetColliderState(IColliderHandle colliderHandle, ColliderWriteState writeState)
        {
            b2Manager.SetColiderState(colliderHandle, writeState);
        }

        public IReadOnlyCollection<BackendCollisionEvent> GetCollisionEvents(IWorldHandle world)
        {
            return b2Manager.GetCollisionEvents(world);
        }
    }
}
