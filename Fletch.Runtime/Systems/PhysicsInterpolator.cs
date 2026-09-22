using Fletch.Core.Components.Update;
using Fletch.Core.Math.Geometry;
using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy;
using Fletch.Engine.Model;
using Fletch.Engine.Scenes;
using Fletch.Engine.Systems;
using Fletch.Physics.Components;
using Fletch.Rendering.Components;
using Fletch.Rendering.Model;
using Fletch.Runtime.Model.Interpolation;
using System.Numerics;

namespace Fletch.Runtime.Systems
{
    internal class PhysicsInterpolator : SceneSubsystem, IUpdateable
    {
        private readonly Dictionary<GameObject, PoseBind> gameObjectPoseBinds = new Dictionary<GameObject, PoseBind>();

        public override void AttachToScene(Scene scene)
        {
            base.AttachToScene(scene);

            scene.SystemManager.RegisterSystem(this, SystemExecutionOrder.RenderPrep, 0);

            scene.AddSystemComponentRegistration(typeof(IInternalRenderingComponent), (b, c) => OnRendererChange(b, c));
            scene.AddSystemComponentRegistration(typeof(RigidBody), (b, c) => OnRigidbodyChange(b, c));
        }

        public void Update(FrameTime frameTime)
        {
            foreach (KeyValuePair<GameObject, PoseBind> gameobjectPoseBind in gameObjectPoseBinds)
            {
                PoseBind poseBind = gameobjectPoseBind.Value;
                GameObject gameObject = gameobjectPoseBind.Key;

                if (poseBind.IsValid)
                {
                    foreach (IInternalRenderingComponent renderingComponent in poseBind.RenderingComponents)
                    {
                        RigidBody rigidBody = poseBind.RigidBody;

                        Vector2 renderPostion = Vector2.Lerp(
                            rigidBody.PreviousPose.Position,
                            rigidBody.CurrentPose.Position,
                            frameTime.Alpha);

                        float rotation = Rotation.LerpRadians(
                            rigidBody.PreviousPose.Rotation,
                            rigidBody.CurrentPose.Rotation,
                            frameTime.Alpha);

                        renderingComponent.SetRenderPose(new RenderPose(renderPostion, rotation));
                    }
                }
                else if(poseBind.RenderingComponents.Count > 0)
                {
                    foreach(IInternalRenderingComponent renderingComponent in poseBind.RenderingComponents)
                    {
                        RenderPose renderPose = new RenderPose(gameObject.Transform.WorldPosition, gameObject.Transform.WorldRotation.Radians);

                        renderingComponent.SetRenderPose(renderPose);
                    }
                }
            }
        }

        private void OnRendererChange(GameObjectComponent component, ComponentChangeType changeType)
        {
            if (component is not IInternalRenderingComponent renderingComponent)
                throw new Exception();

            if(!gameObjectPoseBinds.TryGetValue(component.GameObject, out PoseBind? bind))
            {
                bind = new PoseBind();

                gameObjectPoseBinds[component.GameObject] = bind;
            }

            switch (changeType)
            {
                case ComponentChangeType.Added or ComponentChangeType.Enabled:
                    bind.RenderingComponents.Add(renderingComponent);
                    return;

                case ComponentChangeType.Removed or ComponentChangeType.Disabled:
                    bind.RenderingComponents.Remove(renderingComponent);
                    break;
            }
        }

        private void OnRigidbodyChange(GameObjectComponent component, ComponentChangeType changeType)
        {
            if (component is not RigidBody rigidbody)
                throw new Exception();

            if (!gameObjectPoseBinds.TryGetValue(component.GameObject, out PoseBind? bind))
            {
                bind = new PoseBind();
                
                gameObjectPoseBinds[component.GameObject] = bind;
            }

            switch (changeType)
            {
                case ComponentChangeType.Added or ComponentChangeType.Enabled:
                    if (bind.RigidBody != null)
                        return;

                    bind.RigidBody = rigidbody;
                    return;

                case ComponentChangeType.Removed or ComponentChangeType.Disabled:
                    if (bind.RigidBody == null)
                        return;

                    bind.RigidBody = null;
                    break;
            }
        }
    }
}
