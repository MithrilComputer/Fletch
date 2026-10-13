using Fletch.Core.Colors;
using Fletch.Engine.Hierarchy;
using Fletch.Hosting.Abstractions.Contexts;
using Fletch.Physics.Components;
using Fletch.Physics.Components.CollisionShapes;
using Fletch.Rendering.Components;
using Fletch.Sandbox.Abstractions;
using Fletch.Sandbox.Scripts;
using System.IO;

namespace Fletch.Sandbox
{
    internal sealed class LevelOne : GameLevel
    {
        public LevelOne(IGameContext gameContext) : base(gameContext)
        {
            AddCamera();
            AddBackgroundObjects();
            AddPlayer();
        }

        private void AddBackgroundObjects()
        {

        }

        private void AddPlayer()
        {
            GameObject player = levelScene.CreateGameObject();

            SpriteRenderer sprite = player.AddComponent<SpriteRenderer>();

            sprite.VisualResource.Texture = textureFactory.Load(Path.Combine(pathProvider.AssetFolderDirectory, "Rat.png"));

            player.AddComponent<CharacterController>();

            player.AddComponent<RigidBody>().AddCollider<BoxCollider>();
        }

        private void AddCamera()
        {
            GameObject camera = levelScene.CreateGameObject();

            Camera2D cameraComponent = camera.AddComponent<Camera2D>();

            cameraComponent.SamplerMode = Rendering.Model.SamplerMode.Point;
        }

    }
}