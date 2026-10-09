using Box2D.NET;
using Fletch.Physics.Abstractions.Backends;
using Fletch.Physics.Box2D.Helpers;
using Fletch.Physics.Box2D.Model.ResourceHandles;
using Fletch.Physics.Box2D.Natives;
using Fletch.Physics.Model.Info.Collisions;
using Fletch.Physics.Model.Info.IO;
using Fletch.Physics.Model.ResourceHandles;
using System.Drawing;
using System.Numerics;

namespace Fletch.Physics.Box2D.Backend
{
    internal sealed class Box2DPhysicsBackend : IPhysicsBackend
    {
        private readonly Box2DManager b2Manager;
        
        // running ---------------------

        public void Initialize() { }

        public Box2DPhysicsBackend()
        {
            b2Manager = new Box2DManager();
        }

        public void StepWorld(IWorldHandle world, float deltaTime)
        {
            b2Manager.Step(world, deltaTime);
        }


        // creation and destruction ----

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


        // state management ------------

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
            b2Manager.SetColliderState(colliderHandle, writeState);
        }


        // collision events -------------

        public IReadOnlyCollection<BackendCollisionEvent> GetCollisionEvents(IWorldHandle world)
        {
            return b2Manager.GetCollisionEvents(world);
        }


        // forces and impulses -----------

        public void ApplyForce(IBodyHandle bodyHandle, Vector2 force)
        {
            if(bodyHandle is not Box2DBodyHandle b2bodyHandle)
                throw new ArgumentException("Invalid body handle type. Expected Box2DBodyHandle.", nameof(bodyHandle));

            B2Bodies.b2Body_ApplyForceToCenter(b2bodyHandle.Id, FletchB2Converter.ConvertToB2(force), true);
        }

        public void ApplyForceAt(IBodyHandle bodyHandle, Vector2 force, Vector2 point)
        {
            if (bodyHandle is not Box2DBodyHandle b2bodyHandle)
                throw new ArgumentException("Invalid body handle type. Expected Box2DBodyHandle.", nameof(bodyHandle));

            B2Bodies.b2Body_ApplyForce(b2bodyHandle.Id, FletchB2Converter.ConvertToB2(force), FletchB2Converter.ConvertToB2(point), true);
        }

        public void ApplyImpulse(IBodyHandle bodyHandle, Vector2 impulse)
        {
            if (bodyHandle is not Box2DBodyHandle b2bodyHandle)
                throw new ArgumentException("Invalid body handle type. Expected Box2DBodyHandle.", nameof(bodyHandle));

            B2Bodies.b2Body_ApplyLinearImpulseToCenter(b2bodyHandle.Id, FletchB2Converter.ConvertToB2(impulse), true);
        }

        public void ApplyImpulseAt(IBodyHandle bodyHandle, Vector2 impulse, Vector2 point)
        {
            if (bodyHandle is not Box2DBodyHandle b2bodyHandle)
                throw new ArgumentException("Invalid body handle type. Expected Box2DBodyHandle.", nameof(bodyHandle));

            B2Bodies.b2Body_ApplyLinearImpulse(b2bodyHandle.Id, FletchB2Converter.ConvertToB2(impulse), FletchB2Converter.ConvertToB2(point), true);
        }

        public void ApplyTorque(IBodyHandle bodyHandle, float torque)
        {
            if (bodyHandle is not Box2DBodyHandle b2bodyHandle)
                throw new ArgumentException("Invalid body handle type. Expected Box2DBodyHandle.", nameof(bodyHandle));

            B2Bodies.b2Body_ApplyTorque(b2bodyHandle.Id, torque, true);
        }

        public void ApplyAngularImpulse(IBodyHandle bodyHandle, float angularImpulse)
        {
            if (bodyHandle is not Box2DBodyHandle b2bodyHandle)
                throw new ArgumentException("Invalid body handle type. Expected Box2DBodyHandle.", nameof(bodyHandle));

            B2Bodies.b2Body_ApplyAngularImpulse(b2bodyHandle.Id, angularImpulse, true);
        }
    }
}
