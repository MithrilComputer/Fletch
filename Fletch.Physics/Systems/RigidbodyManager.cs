using Fletch.Engine.Model;
using Fletch.Physics.Abstractions.Backends;
using Fletch.Physics.Components;
using Fletch.Physics.Helpers;
using Fletch.Physics.Model.Info.IO;
using Fletch.Physics.Model.ResourceHandles;

namespace Fletch.Physics.Systems
{
    internal class RigidBodyManager
    {
        private readonly TrackedSet<RigidBody> rigidbodies = new TrackedSet<RigidBody>();

        private IWorldHandle worldHandle;

        private readonly IPhysicsBackend physicsBackend;

        public RigidBodyManager(IPhysicsBackend physicsBackend, IWorldHandle worldHandle)
        {
            this.physicsBackend = physicsBackend;
            this.worldHandle = worldHandle;
        }

        public void UpdateRigidBodies()
        {
            rigidbodies.Refresh();

            foreach (RigidBody rigidBody in rigidbodies.Items)
            {
                if(!rigidBody.Initialized)
                {
                    InitalizeRigidBody(rigidBody);
                }

                if(rigidBody.IsDirty)
                {
                    BodyWriteState writeState = PhysicsHelper.CreateBodyWriteState(rigidBody);
                    physicsBackend.SetBodyState(rigidBody.BodyHandle, writeState);
                    rigidBody.ClearDirty();
                }

                if (!rigidBody.IsEnabled)
                    continue;

                rigidBody.PreviousPose = rigidBody.CurrentPose;

                BodyReadState readState = physicsBackend.GetBodyState(rigidBody.BodyHandle);

                PhysicsHelper.ApplyReadStateToRigidBody(rigidBody, readState);
            }
        }

        public void AddRigidBody(RigidBody rigidbody)
        {
            rigidbodies.MarkToAdd(rigidbody);
        }

        public void RemoveRigidBody(RigidBody rigidbody)
        {
            rigidbodies.MarkToRemove(rigidbody);
        }

        private void InitalizeRigidBody(RigidBody rigidbody)
        {
            BodyWriteState writeState = PhysicsHelper.CreateBodyWriteState(rigidbody);
            IBodyHandle bodyHandle = physicsBackend.CreateBody(worldHandle, writeState);
            rigidbody.Initialize(bodyHandle, this);
        }
    }
}
