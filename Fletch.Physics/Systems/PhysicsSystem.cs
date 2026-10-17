using Fletch.Core.EngineConfig;
using Fletch.Core.LifeCycle;
using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Model;
using Fletch.Engine.Systems;
using Fletch.Engine.Systems.Scenes;
using Fletch.Physics.Abstractions.Backends;
using Fletch.Physics.Components;
using Fletch.Physics.Factories;
using Fletch.Physics.Managers;
using Fletch.Physics.Maps;
using Fletch.Physics.Model.Info.Collisions;
using Fletch.Physics.Model.ResourceHandles;

namespace Fletch.Physics.Systems
{
    internal class PhysicsSystem : SceneSubsystem, IFixedUpdateable, IDisposable
    {
        public IPhysicsBackend PhysicsBackend { get; }

        private readonly IWorldHandle worldHandle;

        private readonly RigidBodyManager rigidBodyManager;

        public readonly ColliderFactory colliderFactory;

        private readonly ColliderRigidbodyMap colliderRigidbodyMap;

        private readonly ColliderManager colliderManager;

        private bool isDisposed = false;

        public PhysicsSystem(IPhysicsBackend backend)
        {
            PhysicsBackend = backend;

            worldHandle = backend.CreateWorld(EngineConfig.Gravity);

            colliderRigidbodyMap = new ColliderRigidbodyMap();

            colliderFactory = new ColliderFactory(backend, colliderRigidbodyMap);

            colliderManager = new ColliderManager(this, backend);

            rigidBodyManager = new RigidBodyManager(backend, worldHandle, this, colliderManager);
        }

        public override void AttachToScene(Scene scene)
        {
            base.AttachToScene(scene);

            scene.SystemManager.RegisterSystem(this, SystemExecutionOrder.Simulation, 0);

            scene.AddSystemComponentRegistration(typeof(RigidBody), (b, c) => OnRigidBodyChange(b, c));
        }

        public void FixedUpdate(FixedTimeStep deltaTime)
        {
            rigidBodyManager.UpdateComponents();

            colliderManager.UpdateColliders();

            rigidBodyManager.WriteRigidBodies();

            rigidBodyManager.ApplyPendingForces();

            PhysicsBackend.StepWorld(worldHandle, deltaTime.Delta * EngineConfig.SimSpeed);

            rigidBodyManager.ReadRigidBodies();

            HandleBackendCollisionEvents();
        }

        private void HandleBackendCollisionEvents()
        {
            foreach(BackendCollisionEvent backendCollisionEvent in PhysicsBackend.GetCollisionEvents(worldHandle))
            {
                colliderManager.HandleCollisionEvent(backendCollisionEvent);
            }
        }

        private void OnRigidBodyChange(GameObjectComponent component, ComponentChangeType changeType)
        {
            if (component is not RigidBody rigidBody)
            {
                throw new InvalidOperationException();
            }

            switch (changeType)
            {
                case ComponentChangeType.Added:

                    rigidBodyManager.AddRigidBody(rigidBody);
                    break;

                case ComponentChangeType.Removed:

                    rigidBodyManager.MarkRemoveRigidBody(rigidBody);
                    break;

                default: // We can ignore other change types, as the rigidbodies track their modifed changes internaly.
                    break;
            }
        }



        public void Dispose()
        {
            rigidBodyManager.Dispose();
        }
    }
}
