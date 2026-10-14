using Fletch.Audio.Components;
using Fletch.Core.Colors;
using Fletch.Engine.Hierarchy;
using Fletch.Hosting.Abstractions.Contexts;
using Fletch.Physics.Components;
using Fletch.Physics.Components.CollisionShapes;
using Fletch.Rendering.Components;
using Fletch.Sandbox.Abstractions;
using Fletch.Sandbox.Scripts;
using System.IO;
using System.Numerics;

namespace Fletch.Sandbox
{
    internal sealed class LevelOne : GameLevel
    {
        private Vector2 playerStartPosition = new Vector2(-5f, 0f);

        public LevelOne(IGameContext gameContext) : base(gameContext)
        {
            AddCamera();
            AddBackgroundObjects();
            AddPlayer();
        }

        private void AddBackgroundObjects()
        {
            GameObject background = levelScene.CreateGameObject();

            SpriteRenderer backgroundSprite = background.AddComponent<SpriteRenderer>();

            backgroundSprite.VisualResource.PixelPerWorldUnit = 16;

            backgroundSprite.VisualResource.Texture = textureFactory.Load(Path.Combine(pathProvider.AssetFolderDirectory, "Background.png"));
        }

        private void AddPlayer()
        {
            GameObject player = levelScene.CreateGameObject();

            player.Transform.LocalPosition = playerStartPosition;

            SpriteRenderer sprite = player.AddComponent<SpriteRenderer>();

            sprite.VisualResource.PixelPerWorldUnit = 16;

            sprite.VisualResource.Texture = textureFactory.CreateSolidColor(8, 8, Color.Red);

            player.AddComponent<CharacterController>();

            player.AddComponent<AudioSource>();

            BoxCollider box = player.AddComponent<RigidBody>().AddCollider<BoxCollider>();

            box.Width = 0.5f;
            box.Height = 0.5f;
        }

        private void AddCamera()
        {
            GameObject camera = levelScene.CreateGameObject();

            Camera2D cameraComponent = camera.AddComponent<Camera2D>();

            camera.AddComponent<AudioListener>();

            cameraComponent.SamplerMode = Rendering.Model.SamplerMode.Point;
        }
    }
}