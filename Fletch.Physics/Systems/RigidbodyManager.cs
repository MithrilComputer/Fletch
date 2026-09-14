using Fletch.Engine.Model;
using Fletch.Physics.Abstractions.Backends;
using Fletch.Physics.Components;
using Fletch.Physics.Helpers;
using Fletch.Physics.Model.Info.IO;
using Fletch.Physics.Model.ResourceHandles;
using System.Diagnostics;
using System.Numerics;

namespace Fletch.Physics.Systems
{
    internal class RigidBodyManager
    {
        private readonly TrackedSet<RigidBody> rigidbodies = new TrackedSet<RigidBody>();

        private IWorldHandle worldHandle;

        private readonly IPhysicsBackend physicsBackend;

        private readonly PhysicsSystem physicsSystem;

        public RigidBodyManager(IPhysicsBackend physicsBackend, IWorldHandle worldHandle, PhysicsSystem physicsSystem)
        {
            this.physicsBackend = physicsBackend;
            this.worldHandle = worldHandle;
            this.physicsSystem = physicsSystem;
        }

        public void UpdateRigidBodies()
        {
            rigidbodies.Refresh();

            foreach (RigidBody rigidBody in rigidbodies.Items)
            {
                if(!rigidBody.Initialized)
                {
                    InitalizeRigidBody(rigidBody);
                    Debug.Print($"RigidBody initialized for GameObject: {rigidBody.GameObject.Name}");
                }

                if (rigidBody.IsDirty)
                {
                    BodyWriteState writeState = PhysicsHelper.CreateBodyWriteState(rigidBody);
                    physicsBackend.SetBodyState(rigidBody.BodyHandle, writeState);
                    rigidBody.ClearDirty();
                    Debug.Print($"RigidBody state updated for GameObject: {rigidBody.GameObject.Name}");
                }

                if (!rigidBody.IsEnabled)
                    continue;

                rigidBody.PreviousPose = rigidBody.CurrentPose;

                BodyReadState readState = physicsBackend.GetBodyState(rigidBody.BodyHandle);

                Debug.Print($"RigidBody data: pos:{readState.Position}, vel:{readState.LinearVelocity}, awk:{readState.IsAwake}");

                PhysicsHelper.ApplyReadStateToRigidBody(rigidBody, readState);

                Debug.Print($"RigidBody state read for GameObject: {rigidBody.GameObject.Name}");
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
            rigidbody.Initialize(bodyHandle, this, physicsSystem);
        }
    }
}
