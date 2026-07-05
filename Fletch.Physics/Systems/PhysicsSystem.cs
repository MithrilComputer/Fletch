using Fletch.Engine.Components;
using Fletch.Engine.Model;
using Fletch.Engine.Scenes;
using Fletch.Engine.Systems;
using Fletch.Physics.Components;
using Fletch.Physics.Components.CollisionShapes;

namespace Fletch.Physics.Systems
{
    internal class PhysicsSystem : SceneSubsystem
    {
        public override void AttachToScene(Scene scene)
        {
            base.AttachToScene(scene);

            scene.SystemManager.RegisterSystem(this, SystemExecutionOrder.Simulation, 0);

            scene.AddSystemComponentRegistration(typeof(RigidBody), (b, c) => OnRigidBodyChange(b, c));
            scene.AddSystemComponentRegistration(typeof(BoxCollider), (b, c) => OnColliderChange(b, c));
        }

        private void OnRigidBodyChange(GameObjectComponent component, ComponentChangeType changeType)
        {

        }

        private void OnColliderChange(GameObjectComponent component, ComponentChangeType changeType)
        {

        }
    }
}
