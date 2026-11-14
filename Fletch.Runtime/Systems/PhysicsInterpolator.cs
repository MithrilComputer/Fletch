using Fletch.Core.LifeCycle;
using Fletch.Core.Math.Geometry;
using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy.Objects;
using Fletch.Engine.Model;
using Fletch.Engine.Systems;
using Fletch.Engine.Systems.Scenes;
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

            scene.AddSystemComponentRegistration(typeof(IInternalWorldRenderingComponent), (b, c) => OnRendererChange(b, c));
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
                    foreach (IInternalWorldRenderingComponent renderingComponent in poseBind.RenderingComponents)
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

                        gameObject.Transform.LocalPosition = renderPostion;
                        gameObject.Transform.LocalRotation.SetRadians(rotation);
                    }
                }
                else if (poseBind.RenderingComponents.Count > 0)
                {
                    foreach (IInternalWorldRenderingComponent renderingComponent in poseBind.RenderingComponents)
                    {
                        RenderPose renderPose = new RenderPose(gameObject.Transform.WorldPosition, gameObject.Transform.WorldRotation.Radians);

                        renderingComponent.SetRenderPose(renderPose);
                    }
                }
            }
        }

        private void OnRendererChange(Component component, ComponentChangeType changeType)
        {
            if (component is not IInternalWorldRenderingComponent renderingComponent)
                throw new Exception();

            GameObject gameObject = ((GameObjectComponent)component).GameObject;

            if (!gameObjectPoseBinds.TryGetValue(gameObject, out PoseBind? bind))
            {
                bind = new PoseBind();

                gameObjectPoseBinds[gameObject] = bind;
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

        private void OnRigidbodyChange(Component component, ComponentChangeType changeType)
        {
            if (component is not RigidBody rigidbody)
                throw new Exception();

            if (!gameObjectPoseBinds.TryGetValue(rigidbody.GameObject, out PoseBind? bind))
            {
                bind = new PoseBind();

                gameObjectPoseBinds[rigidbody.GameObject] = bind;
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
