using Fletch.Audio.Components;
using Fletch.Core.Colors;
using Fletch.Core.Time;
using Fletch.Engine.Hierarchy;
using Fletch.Hosting.Abstractions.Contexts;
using Fletch.Physics.Components;
using Fletch.Physics.Components.CollisionShapes;
using Fletch.Physics.Model;
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

        private readonly WorldScrollingSystem scrollingSystem;

        public LevelOne(IGameContext gameContext) : base(gameContext)
        {
            AddCamera();
            AddBackgroundObjects();
            AddGroundObject();
            AddPlayer();

            scrollingSystem = new WorldScrollingSystem(levelScene, textureFactory);

            levelScene.OnUpdate += Update;
            levelScene.OnFixedUpdate += FixedUpdate;
        }

        private void Update(FrameTime frameTime)
        {
            scrollingSystem.UpdateWorld(frameTime.Delta);
        }

        private void FixedUpdate(FixedTimeStep fixedTime)
        {

        }

        private void AddBackgroundObjects()
        {
            GameObject background = levelScene.CreateGameObject();

            SpriteRenderer backgroundSprite = background.AddComponent<SpriteRenderer>();

            backgroundSprite.VisualResource.PixelPerWorldUnit = 16;

            backgroundSprite.VisualResource.Texture = textureFactory.Load(Path.Combine(pathProvider.AssetFolderDirectory, "Background.png"));
        }

        private void AddGroundObject()
        {
            Vector2 size = new Vector2(20f, 1f);

            Vector2 startLocation = new Vector2(0f, -4.1f);

            GameObject ground = levelScene.CreateGameObject();

            ground.Transform.LocalPosition = startLocation;

            RigidBody body = ground.AddComponent<RigidBody>();

            body.PhysicsMode = PhysicsMode.Static;

            BoxCollider box = body.AddCollider<BoxCollider>();

            box.Width = size.X;
            box.Height = size.Y;
        }

        private void AddPlayer()
        {
            GameObject player = levelScene.CreateGameObject();

            player.Transform.LocalPosition = playerStartPosition;

            SpriteRenderer sprite = player.AddComponent<SpriteRenderer>();

            sprite.VisualResource.PixelPerWorldUnit = 16;

            sprite.VisualResource.Texture = textureFactory.CreateSolidColor(8, 8, Color.Red);

            CharacterController charaterControler = player.AddComponent<CharacterController>();

            CharaterAnimation animationComp = player.AddComponent<CharaterAnimation>();

            charaterControler.OnJump += animationComp.OnJump;

            player.AddComponent<AudioSource>();

            RigidBody playerRb = player.AddComponent<RigidBody>();

            playerRb.UseInterpolation = true;

            playerRb.FixedRotation = true;

            BoxCollider box = playerRb.AddCollider<BoxCollider>();

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