using Fletch.Audio.Components;
using Fletch.Audio.Model;
using Fletch.Core.Time;
using Fletch.Engine.Hierarchy.Objects;
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


        double lowestFrameTime = double.MaxValue;

        double highestFrameTime = 0d;


        private CharacterController playerControler;

        private WorldScrollingSystem worldScroller;


        private Camera2D camera2D;

        private IGameContext gameContext;

        private readonly Process process = Process.GetCurrentProcess();

        private const int MaxFrameSamples = 16384;
        private readonly double[] frameTimes = new double[MaxFrameSamples];
        private double totalFrameTime;

        private long previousAllocatedBytes;
        private long previousGen0;
        private long previousGen1;
        private long previousGen2;

        private TimeSpan previousCpuTime;
        private bool profilerInitialized;

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

            MonitorPerformance();
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

        private void MonitorPerformance()
        {
            //TODO Made by AI, Replace later

            double frameTimeMs = frameTimer.Elapsed.TotalMilliseconds;
            frameTimer.Restart();

            if (!profilerInitialized)
            {
                previousAllocatedBytes = GC.GetAllocatedBytesForCurrentThread();

                previousGen0 = GC.CollectionCount(0);
                previousGen1 = GC.CollectionCount(1);
                previousGen2 = GC.CollectionCount(2);

                previousCpuTime = process.TotalProcessorTime;

                profilerInitialized = true;
                return;
            }

            if (frames < MaxFrameSamples)
            {
                frameTimes[frames] = frameTimeMs;
            }

            frames++;

            totalFrameTime += frameTimeMs;

            lowestFrameTime = Math.Min(lowestFrameTime, frameTimeMs);
            highestFrameTime = Math.Max(highestFrameTime, frameTimeMs);

            if (totalFrameTime < 1000.0)
                return;

            double elapsedSeconds = totalFrameTime / 1000.0;

            double fps = frames / elapsedSeconds;
            double averageFrameTime = totalFrameTime / frames;

            int sampleCount = Math.Min(frames, MaxFrameSamples);

            Array.Sort(frameTimes, 0, sampleCount);

            double p95 = GetPercentile(frameTimes, sampleCount, 0.95);
            double p99 = GetPercentile(frameTimes, sampleCount, 0.99);
            double p999 = GetPercentile(frameTimes, sampleCount, 0.999);

            double onePercentLow = p99 > 0 ? 1000.0 / p99 : 0;
            double pointOnePercentLow = p999 > 0 ? 1000.0 / p999 : 0;

            long allocatedBytes = GC.GetAllocatedBytesForCurrentThread();

            long allocatedDelta = allocatedBytes - previousAllocatedBytes;

            double allocatedPerSecond = allocatedDelta / elapsedSeconds;
            double allocatedPerFrame = (double)allocatedDelta / frames;

            long gen0 = GC.CollectionCount(0);
            long gen1 = GC.CollectionCount(1);
            long gen2 = GC.CollectionCount(2);

            long gen0Collections = gen0 - previousGen0;
            long gen1Collections = gen1 - previousGen1;
            long gen2Collections = gen2 - previousGen2;

            long managedMemory = GC.GetTotalMemory(false);

            process.Refresh();
            long workingSet = process.WorkingSet64;
            long privateMemory = process.PrivateMemorySize64;

            TimeSpan currentCpuTime = process.TotalProcessorTime;
            double cpuSeconds = (currentCpuTime - previousCpuTime).TotalSeconds;

            double cpuUsage =
                cpuSeconds / (elapsedSeconds * Environment.ProcessorCount) * 100.0;

            Debug.WriteLine(
                $"\n========== FLETCH PERFORMANCE ==========\n" +
                $"FPS:                  {fps:F0}\n" +
                $"Average Frame Time:   {averageFrameTime:F4} ms\n" +
                $"Minimum Frame Time:   {lowestFrameTime:F4} ms\n" +
                $"Maximum Frame Time:   {highestFrameTime:F4} ms\n" +
                $"95th Percentile:      {p95:F4} ms\n" +
                $"99th Percentile:      {p99:F4} ms\n" +
                $"1% Low FPS:           {onePercentLow:F0}\n" +
                $"0.1% Low FPS:         {pointOnePercentLow:F0}\n" +
                $"----------------------------------------\n" +
                $"Allocated / Frame:    {allocatedPerFrame:F0} B\n" +
                $"Allocated / Second:   {allocatedPerSecond / 1048576.0:F3} MiB\n" +
                $"Managed Memory:       {managedMemory / 1048576.0:F2} MiB\n" +
                $"Working Set:          {workingSet / 1048576.0:F2} MiB\n" +
                $"Private Memory:       {privateMemory / 1048576.0:F2} MiB\n" +
                $"----------------------------------------\n" +
                $"Gen 0 Collections:    {gen0Collections}\n" +
                $"Gen 1 Collections:    {gen1Collections}\n" +
                $"Gen 2 Collections:    {gen2Collections}\n" +
                $"CPU Usage:            {cpuUsage:F2}%\n" +
                $"========================================"
            );

            previousAllocatedBytes = GC.GetAllocatedBytesForCurrentThread();

            previousGen0 = GC.CollectionCount(0);
            previousGen1 = GC.CollectionCount(1);
            previousGen2 = GC.CollectionCount(2);

            previousCpuTime = process.TotalProcessorTime;

            frames = 0;
            totalFrameTime = 0;
            lowestFrameTime = double.MaxValue;
            highestFrameTime = 0;

            frameTimer.Restart();
        }

        private static double GetPercentile(
            double[] sortedSamples,
            int count,
            double percentile)
        {
            if (count == 0)
                return 0;

            double index = (count - 1) * percentile;

            int lower = (int)Math.Floor(index);
            int upper = (int)Math.Ceiling(index);

            double fraction = index - lower;

            return sortedSamples[lower] +
                (sortedSamples[upper] - sortedSamples[lower]) * fraction;
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