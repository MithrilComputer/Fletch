using Fletch.Audio.Components;
using Fletch.Core.Time;
using Fletch.Engine.Hierarchy;
using Fletch.Hosting.Abstractions.Contexts;
using Fletch.Physics.Components;
using Fletch.Physics.Components.CollisionShapes;
using Fletch.Physics.Model;
using Fletch.Rendering.Abstractions.Factories;
using Fletch.Rendering.Components;
using Fletch.Sandbox.Abstractions;
using Fletch.Sandbox.Scripts;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;

namespace Fletch.Sandbox
{
    internal sealed class LevelOne : GameLevel
    {
        private readonly WorldScrollingSystem scrollingSystem;

        
        private float timeKeep = 0f;

        private int frames = 0;

        
        private readonly Stopwatch frameTimer = Stopwatch.StartNew();


        double lowestFrameTime = 1000000d;

        double highestFrameTime = 0d;


        private CharacterController playerControler;

        private WorldScrollingSystem worldScroller;

        public LevelOne(IGameContext gameContext) : base(gameContext)
        {
            AddCamera();
            AddBackgroundObjects();
            AddGroundObject();
            AddPlayer();

            scrollingSystem = new WorldScrollingSystem(levelScene, textureFactory, gameContext.PathProvider);

            levelScene.OnUpdate += Update;
            levelScene.OnFixedUpdate += FixedUpdate;

            GameObject gameManager = levelScene.CreateGameObject();

            gameManager.AddComponent<GameManager>().Initalize(playerControler, AddUIElements(), scrollingSystem);
        }

        private void Update(FrameTime frameTime)
        {

            scrollingSystem.UpdateWorld(frameTime.Delta);

            MonitorPreformance(frameTime.Delta);
        }

        private void MonitorPreformance(float delta)
        {
            timeKeep += delta;

            if (timeKeep >= 1f)
            {
                Debug.WriteLine($"FPS: {frames}, Average Frame Time: {timeKeep * 1000d / frames}ms, Lowest Frame Time: {lowestFrameTime}ms, Highest Frame Time: {highestFrameTime}ms");

                highestFrameTime = 0d;
                lowestFrameTime = 1000d;

                timeKeep = 0f;

                frames = 0;
            }

            double frameTimeMs = frameTimer.Elapsed.TotalMilliseconds;
            frameTimer.Restart();

            if (frameTimeMs > highestFrameTime)
            {
                highestFrameTime = frameTimeMs;
            }

            if (frameTimeMs < lowestFrameTime)
            {
                lowestFrameTime = frameTimeMs;
            }

            frames++;
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

        private List<UIComponent> AddUIElements()
        {
            List<UIComponent> uiComps = new List<UIComponent>();

            GameObject startScreen = levelScene.CreateGameObject();

            UIComponent ui = startScreen.AddComponent<UIComponent>();

            SpriteRenderer renderer = startScreen.AddComponent<SpriteRenderer>();

            renderer.VisualResource.PixelPerWorldUnit = 16;

            renderer.VisualResource.Texture = textureFactory.Load(Path.Combine(pathProvider.AssetFolderDirectory, "Start Screen.png"));

            renderer.ZHeight = 1;

            uiComps.Add(ui);

            return uiComps;
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

            SpriteRenderer sprite = player.AddComponent<SpriteRenderer>();

            sprite.VisualResource.PixelPerWorldUnit = 16;

            CharacterController charaterControler = player.AddComponent<CharacterController>();

            playerControler = charaterControler;

            CharaterAnimation animationComp = player.AddComponent<CharaterAnimation>();

            charaterControler.OnJump += animationComp.OnJump;

            player.AddComponent<AudioSource>();

            RigidBody playerRb = player.AddComponent<RigidBody>();

            playerRb.UseInterpolation = true;

            playerRb.FixedRotation = true;

            playerRb.LockX = true;

            CircleCollider circle = playerRb.AddCollider<CircleCollider>();

            circle.Radius = 0.25f;

            circle.Mass = 1.21f;
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