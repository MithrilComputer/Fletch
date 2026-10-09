using Fletch.Engine.Model;
using Fletch.Physics.Abstractions.Backends;
using Fletch.Physics.Abstractions.CollisionShapes;
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

            foreach (RigidBody rigidBody in rigidbodies.Items)
            {
                rigidBody.UpdateColliders();
            }
        }

        public void UpdateColliderState(Collider collider)
        {
            physicsBackend.SetColliderState(collider.ColliderHandle, PhysicsHelper.CreateColliderWriteState(collider));
        }

        public void AddRigidBody(RigidBody rigidbody)
        {
            rigidbodies.MarkToAdd(rigidbody);
            InitializeRigidBody(rigidbody);
        }

        /// <summary>
        /// Removes a rigid body from the manager and marks it for removal from the physics backend.
        /// </summary>
        /// <param name="rigidbody">The rigid body to remove.</param>
        public void RemoveRigidBody(RigidBody rigidbody)
        {
            rigidbodies.MarkToRemove(rigidbody);
        }

        /// <summary>
        /// Initializes the rigid body by creating a corresponding body in the physics backend and setting its initial pose.
        /// </summary>
        /// <param name="rigidbody">The rigid body to initialize.</param>
        private void InitializeRigidBody(RigidBody rigidbody)
        {
            Vector2 pos = rigidbody.GameObject.Transform.WorldPosition;
            float rot = rigidbody.GameObject.Transform.WorldRotation.Radians;

            rigidbody.CurrentPose = new PhysicsPose(pos, rot);

            BodyWriteState writeState = PhysicsHelper.CreateBodyWriteState(rigidbody);
            IBodyHandle bodyHandle = physicsBackend.CreateBody(worldHandle, writeState);
            
            rigidbody.Initialize(bodyHandle, this, physicsSystem);
        }


        /// <summary>
        /// Applies a force to the rigid body.
        /// </summary>
        /// <param name="rigidBody">The rigid body to apply the force to.</param>
        /// <param name="force">The force to apply.</param>
        public void ApplyForceToBody(RigidBody rigidBody, Vector2 force)
        {
            physicsBackend.ApplyForce(rigidBody.BodyHandle, force);
        }

        /// <summary>
        /// Applies a force to the rigid body at a specific point in world space. The point is relative to the rigid body's position.
        /// </summary>
        /// <param name="rigidBody">The rigid body to apply the force to.</param>
        /// <param name="force">The force to apply.</param>
        /// <param name="point">The point at which to apply the force, relative to the rigid body's position.</param>
        public void ApplyForceToBodyAt(RigidBody rigidBody, Vector2 force, Vector2 point)
        {
            physicsBackend.ApplyForceAt(rigidBody.BodyHandle, force, point + rigidBody.CurrentPose.Position);
        }

        /// <summary>
        /// Applies an impulse to a specified rigid body.
        /// </summary>
        /// <param name="rigidBody">The rigid body to apply the impulse to.</param>
        /// <param name="impulse">The impulse vector to apply.</param>
        public void ApplyImpulseToBody(RigidBody rigidBody, Vector2 impulse)
        {
            physicsBackend.ApplyImpulse(rigidBody.BodyHandle, impulse);
        }

        /// <summary>
        /// Applies an impulse to the rigid body at a specific point in world space. The point is relative to the rigid body's position.
        /// </summary>
        /// <param name="rigidBody">The rigid body to apply the impulse to.</param>
        /// <param name="impulse">The impulse vector to apply.</param>
        /// <param name="point">The point at which to apply the impulse, relative to the rigid body's position.</param>
        public void ApplyImpulseToBodyAt(RigidBody rigidBody, Vector2 impulse, Vector2 point)
        {
            physicsBackend.ApplyImpulseAt(rigidBody.BodyHandle, impulse, point + rigidBody.CurrentPose.Position);
        }

        /// <summary>
        /// Applies a torque to the rigid body.
        /// </summary>
        /// <param name="rigidBody">The rigid body to apply the torque to.</param>
        /// <param name="torque">The torque to apply.</param>
        public void ApplyTorqueToBody(RigidBody rigidBody, float torque)
        {
            physicsBackend.ApplyTorque(rigidBody.BodyHandle, torque);
        }

        /// <summary>
        /// Applies an angular impulse to the rigid body.
        /// </summary>
        /// <param name="rigidBody">The rigid body to apply the angular impulse to.</param>
        /// <param name="angularImpulse">The angular impulse to apply.</param>
        public void ApplyAngularImpulseToBody(RigidBody rigidBody, float angularImpulse)
        {
            physicsBackend.ApplyAngularImpulse(rigidBody.BodyHandle, angularImpulse);
        }
    }
}
