using Fletch.Engine.Model;
using Fletch.Physics.Abstractions.Backends;
using Fletch.Physics.Components;
using Fletch.Physics.Helpers;
using Fletch.Physics.Model;
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

        public void WriteRigidBodies()
        {
            foreach (RigidBody rigidBody in rigidbodies.Items)
            {
                if (!rigidBody.Initialized)
                {
                    InitializeRigidBody(rigidBody);
                    continue;
                }

                if (!rigidBody.IsEnabled)
                    continue;

                if (rigidBody.IsDirty)
                {
                    BodyWriteState writeState = PhysicsHelper.CreateBodyWriteState(rigidBody);
                    physicsBackend.SetBodyState(rigidBody.BodyHandle, writeState);
                    rigidBody.ClearDirty();
                    Debug.Print($"RigidBody state updated for GameObject: {rigidBody.GameObject.Name}");
                }
            }
        }

        public void ReadRigidBodies()
        {
            foreach (RigidBody rigidBody in rigidbodies.Items)
            {
                if (!rigidBody.Initialized)
                {
                    InitializeRigidBody(rigidBody);
                    continue;
                }

                if (!rigidBody.IsEnabled)
                    continue;

                rigidBody.PreviousPose.Position = rigidBody.CurrentPose.Position;
                rigidBody.PreviousPose.Rotation = rigidBody.CurrentPose.Rotation;

                BodyReadState readState = physicsBackend.GetBodyState(rigidBody.BodyHandle);

                PhysicsHelper.ApplyReadStateToRigidBody(rigidBody, readState);
            }
        }

        public void UpdateComponents()
        {
            rigidbodies.Refresh();
        }

        public void AddRigidBody(RigidBody rigidbody)
        {
            rigidbodies.MarkToAdd(rigidbody);
            InitializeRigidBody(rigidbody);
        }

        public void RemoveRigidBody(RigidBody rigidbody)
        {
            rigidbodies.MarkToRemove(rigidbody);
        }

        private void InitializeRigidBody(RigidBody rigidbody)
        {
            Vector2 pos = rigidbody.GameObject.Transform.WorldPosition;
            float rot = rigidbody.GameObject.Transform.WorldRotation.Radians;

            rigidbody.CurrentPose = new PhysicsPose(pos, rot);

            BodyWriteState writeState = PhysicsHelper.CreateBodyWriteState(rigidbody);
            IBodyHandle bodyHandle = physicsBackend.CreateBody(worldHandle, writeState);
            
            rigidbody.Initialize(bodyHandle, this, physicsSystem);
        }

        public void OnApplyImpulse(RigidBody rigidBody, Vector2 impulse)
        {
            physicsBackend.ImpulseBody(rigidBody.BodyHandle, impulse, rigidBody.CurrentPose.Position);
        }
    }
}
