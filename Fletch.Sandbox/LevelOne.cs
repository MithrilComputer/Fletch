using Fletch.Audio.Components;
using Fletch.Audio.Model;
using Fletch.Core.Time;
using Fletch.Engine.Hierarchy;
using Fletch.Hosting.Abstractions.Contexts;
using Fletch.Physics.Components;
using Fletch.Physics.Components.CollisionShapes;
using Fletch.Physics.Model;
using Fletch.Rendering.Components;
using Fletch.Sandbox.Abstractions;
using Fletch.Sandbox.Scripts;
using System;
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


        private Camera2D camera2D;

        private IGameContext gameContext;


        public LevelOne(IGameContext gameContext) : base(gameContext)
        {
            this.gameContext = gameContext;

            AddCamera();
            AddBackgroundObjects();
            AddBoundaryObjects();
            AddPlayer();
            AddMusic();

            scrollingSystem = new WorldScrollingSystem(levelScene, textureFactory, gameContext.PathProvider);

            levelScene.OnUpdate += Update;

            GameObject gameManager = levelScene.CreateGameObject();

            gameManager.AddComponent<GameManager>().Initalize(playerControler, AddUIElements(), scrollingSystem);
        }

        private void Update(FrameTime frameTime)
        {
            UpdateZoom(input.Mouse.WheelDeltaNormalized);

            scrollingSystem.UpdateWorld(frameTime.Delta);

            MonitorPreformance(frameTime.Delta);
        }

        private void UpdateZoom(float scrollInput)
        {
            const float zoomSpeed = 0.1f;
            const float minZoom = 0.02f;
            const float maxZoom = 5f;

            camera2D.Zoom = Math.Clamp(
                camera2D.Zoom * (1f + scrollInput * zoomSpeed),
                minZoom,
                maxZoom
            );
        }

        private void AddMusic()
        {
            GameObject musicPlayerObject = levelScene.CreateGameObject();

            AudioSource audioSource = musicPlayerObject.AddComponent<AudioSource>();

            audioSource.TryCreateSoundPlayer(
                Path.Combine(gameContext.PathProvider.AssetFolderDirectory, "ChiptuneB.wav"),
                out SoundPlayer? musicPlayer);

            musicPlayer.Volume = 0.3f;

            musicPlayer.IsLooping = true;

            musicPlayer.PlaySound();
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

        private void AddBackgroundObjects()
        {
            GameObject background = levelScene.CreateGameObject();

            SpriteRenderer backgroundSprite = background.AddComponent<SpriteRenderer>();

            backgroundSprite.VisualResource.PixelPerWorldUnit = 16;

            backgroundSprite.ZHeight = 0;

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

            renderer.ZHeight = 3;

            uiComps.Add(ui);

            return uiComps;
        }

        private void AddBoundaryObjects()
        {
            Vector2 groundSize = new Vector2(20f, 1f);

            Vector2 groundStartLocation = new Vector2(0f, -4.1f);

            GameObject ground = levelScene.CreateGameObject();

            ground.Transform.LocalPosition = groundStartLocation;

            RigidBody groundBody = ground.AddComponent<RigidBody>();

            groundBody.PhysicsMode = PhysicsMode.Static;

            BoxCollider groundBox = groundBody.AddCollider<BoxCollider>();

            groundBox.Width = groundSize.X;
            groundBox.Height = groundSize.Y;


            Vector2 ceillingSize = new Vector2(20f, 1f);

            Vector2 ceillingStartLocation = new Vector2(0f, 6.125f);

            GameObject ceilling = levelScene.CreateGameObject();

            ceilling.Transform.LocalPosition = ceillingStartLocation;

            RigidBody ceillingBody = ceilling.AddComponent<RigidBody>();

            ceillingBody.PhysicsMode = PhysicsMode.Static;

            BoxCollider ceillingBox = ceillingBody.AddCollider<BoxCollider>();

            ceillingBox.Width = ceillingSize.X;
            ceillingBox.Height = ceillingSize.Y;
        }

        private void AddPlayer()
        {
            GameObject player = levelScene.CreateGameObject();

            SpriteRenderer sprite = player.AddComponent<SpriteRenderer>();

            sprite.VisualResource.PixelPerWorldUnit = 16;

            sprite.ZHeight = 2;

            CharacterController charaterControler = player.AddComponent<CharacterController>();

            playerControler = charaterControler;

            CharaterAnimation animationComp = player.AddComponent<CharaterAnimation>();

            charaterControler.OnJump += animationComp.OnJump;

            player.AddComponent<AudioSource>();

            RigidBody playerRb = player.AddComponent<RigidBody>();

            playerRb.UseInterpolation = true;

            playerRb.FixedRotation = true;

            playerRb.LockX = false;

            CircleCollider circle = playerRb.AddCollider<CircleCollider>();

            circle.Material = new PhysicsMaterial(0.1f, 0.2f);

            circle.Radius = 0.25f;

            circle.Mass = 1.21f;
        }

        private void AddCamera()
        {
            GameObject camera = levelScene.CreateGameObject();

            camera2D = camera.AddComponent<Camera2D>();

            camera.AddComponent<AudioListener>();

            camera2D.SamplerMode = Rendering.Model.SamplerMode.Point;
        }
    }
}