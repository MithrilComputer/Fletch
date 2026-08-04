using Fletch.Engine.Model;
using Fletch.Physics.Abstractions.Backends;
using Fletch.Physics.Components;
using Fletch.Physics.Model.Info.IO;

namespace Fletch.Physics.Systems
{
    internal class RigidBodyManager
    {
        private readonly TrackedSet<RigidBody> rigidbodies = new TrackedSet<RigidBody>();

        private readonly IPhysicsBackend physicsBackend;

        public RigidBodyManager(IPhysicsBackend physicsBackend)
        {

            this.physicsBackend = physicsBackend;

        }

        public void UpdateRigidBodies()
        {
            foreach (RigidBody rigidBody in rigidbodies.Items)
            {
                BodyReadState readState = physicsBackend.GetBodyState(rigidBody.BodyHandle);

                rigidBody.GameObject.Transform.LocalPosition = readState.Position;
                rigidBody.GameObject.Rotation = readState.Rotation;
            }
        }

        public void AddRigidBody(RigidBody rigidbody)
        {

        }

        public void RemoveRigidBody(RigidBody rigidbody)
        {

        }

    }
}
