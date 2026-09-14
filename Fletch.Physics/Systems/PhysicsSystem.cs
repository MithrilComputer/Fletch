using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Components.Updateable;
using Fletch.Engine.Model;
using Fletch.Engine.Scenes;
using Fletch.Engine.Systems;
using Fletch.Physics.Abstractions.Backends;
using Fletch.Physics.Components;
using Fletch.Physics.Factories;
using Fletch.Physics.Maps;
using Fletch.Physics.Model.ResourceHandles;
using System.Numerics;

namespace Fletch.Physics.Systems
{
    internal class PhysicsSystem : SceneSubsystem, IFixedUpdateable
    {
        private readonly IPhysicsBackend backend;

        private readonly IWorldHandle worldHandle;

        private readonly RigidBodyManager rigidBodyManager;

        public readonly ColliderFactory colliderFactory;

        private readonly ColliderRigidbodyMap colliderRigidbodyMap;

        public PhysicsSystem(IPhysicsBackend backend)
        {
            this.backend = backend;

            worldHandle = backend.CreateWorld(new Vector2(0, -9.81f));

            colliderRigidbodyMap = new ColliderRigidbodyMap();

            colliderFactory = new ColliderFactory(backend, colliderRigidbodyMap);

            rigidBodyManager = new RigidBodyManager(backend, worldHandle, this);
        }

        public override void AttachToScene(Scene scene)
        {
            base.AttachToScene(scene);

            scene.SystemManager.RegisterSystem(this, SystemExecutionOrder.Simulation, 0);

            scene.AddSystemComponentRegistration(typeof(RigidBody), (b, c) => OnRigidBodyChange(b, c));
        }

        public void FixedUpdate(FixedTimeStep deltaTime)
        {
            rigidBodyManager.UpdateRigidBodies();
            backend.StepWorld(worldHandle, deltaTime.Delta);
        }

        private void OnRigidBodyChange(GameObjectComponent component, ComponentChangeType changeType)
        {
            if(component is not RigidBody rigidBody)
            {
                throw new InvalidOperationException();
            }

            switch(changeType)
            {
                case ComponentChangeType.Added:

                    rigidBodyManager.AddRigidBody(rigidBody);
                    break;

                case ComponentChangeType.Removed:

                    rigidBodyManager.RemoveRigidBody(rigidBody);
                    break;
                
                default: // We can ignore other change types, as the rigidbodies track their modifed changes internaly.
                    break;
            }
        }
    }
}
