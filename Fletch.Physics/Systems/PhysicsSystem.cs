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
    internal class PhysicsSystem : SceneSubsystem, IFixedUpdateable, IUpdateable
    {
        private readonly IPhysicsBackend backend;

        public PhysicsSystem(IPhysicsBackend backend)
        {
            this.backend = backend;
        }

        public override void AttachToScene(Scene scene)
        {
            base.AttachToScene(scene);

            scene.SystemManager.RegisterSystem(this, SystemExecutionOrder.Simulation, 0);

            scene.AddSystemComponentRegistration(typeof(RigidBody), (b, c) => OnRigidBodyChange(b, c));
            scene.AddSystemComponentRegistration(typeof(BoxCollider), (b, c) => OnColliderChange(b, c));
        }


        public void Update(float deltaTime)
        {
            throw new NotImplementedException();
        }

        public void FixedUpdate(FixedTimeStep deltaTime)
        {
            throw new NotImplementedException();
        }

        private void OnRigidBodyChange(GameObjectComponent component, ComponentChangeType changeType)
        {

        }

        private void OnColliderChange(GameObjectComponent component, ComponentChangeType changeType)
        {

        }
    }
}
