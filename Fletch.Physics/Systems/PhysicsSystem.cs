using Fletch.Core.Components.Update;
using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Components.Updateable;
using Fletch.Engine.Model;
using Fletch.Engine.Scenes;
using Fletch.Engine.Systems;
using Fletch.Physics.Abstractions.Backends;
using Fletch.Physics.Components;
using Fletch.Physics.Components.CollisionShapes;
using Fletch.Physics.Model.ResourceHandles;

namespace Fletch.Physics.Systems
{
    internal class PhysicsSystem : SceneSubsystem, IFixedUpdateable
    {
        private readonly IPhysicsBackend backend;

        private readonly IWorldHandle worldHandle;

        public PhysicsSystem(IPhysicsBackend backend)
        {
            this.backend = backend;

            worldHandle = backend.CreateWorld();
        }

        public override void AttachToScene(Scene scene)
        {
            base.AttachToScene(scene);

            scene.SystemManager.RegisterSystem(this, SystemExecutionOrder.Simulation, 0);

            scene.AddSystemComponentRegistration(typeof(RigidBody), (b, c) => OnRigidBodyChange(b, c));
            scene.AddSystemComponentRegistration(typeof(BoxCollider), (b, c) => OnColliderChange(b, c));
        }

        public void FixedUpdate(FixedTimeStep deltaTime)
        {
            
        }

        private void OnRigidBodyChange(GameObjectComponent component, ComponentChangeType changeType)
        {

        }
        private void OnColliderChange(GameObjectComponent component, ComponentChangeType changeType)
        {

        }
    }
}
