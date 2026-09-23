using Fletch.Audio.Components;
using Fletch.Audio.Model;
using Fletch.Audio.Systems;
using Fletch.Core.Colors;
using Fletch.Core.EngineConfig;
using Fletch.Core.Time;
using Fletch.Engine.Abstractions.Factories;
using Fletch.Engine.Hierarchy;
using Fletch.Engine.Scenes;
using Fletch.Input.Abstractions.Backends;
using Fletch.Input.Abstractions.InputDevices;
using Fletch.Input.Model;
using Fletch.Physics.Components;
using Fletch.Physics.Components.CollisionShapes;
using Fletch.Physics.Model;
using Fletch.Physics.Systems;
using Fletch.Platform.Abstractions.Contexts;
using Fletch.Platform.Abstractions.Lifecycle;
using Fletch.Platform.Abstractions.Paths;
using Fletch.Platform.Abstractions.Window;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.Components;
using Fletch.Rendering.Systems;
using Fletch.Runtime.Abstractions.Hosting;
using Fletch.Runtime.Systems;
using System.Diagnostics;
using System.Numerics;

namespace Fletch.Runtime.Hosting
{
    /// <summary>
    /// Setup just for debuging!!! Ignore this, it will be removed in the future. This is just to test the engine and make sure everything is working as expected.
    /// </summary>
    internal class FletchRuntime : IRuntime
    {
        private readonly IWindow window;

        private readonly IApplicationLifetime applicationLifeTime;

        private readonly IPathProvider pathProvider;

        private readonly IRenderingBackend renderingBackend;

        private readonly IInputBackend inputBackend;

        private readonly ISceneFactory sceneFactory;

        private readonly ISubSystemFactory subSystemFactory;

        
        private SpriteRenderer[] visuals;

        private RigidBody[] bodies;


        public IPlatformContext Platform => throw new NotImplementedException();

        public bool IsInitialized { get; private set; }

        //Testing

        private SoundPlayer TestSound;

        private Scene testScene;

        private GameObject gameObject;

        GameObject wallOne;

        GameObject wallTwo;

        private GameObject cameraObject;

        private Camera2D camera;

        private IGamepad gamepad;

        private IKeyboard keyboard;

        private IMouse mouse;

        private float moveSpeed = 5f;

        private float cameraSmooth = 10f;

        private float timeKeep = 0f;

        private int frames = 0;

        Random random = new Random();

        RigidBody rb;

        public FletchRuntime(IPlatformContext platformContext, ISceneFactory sceneFactory, ISubSystemFactory subSystemFactory)
        {
            window = platformContext.Window;
            applicationLifeTime = platformContext.Lifetime;
            pathProvider = platformContext.PathProvider;
            renderingBackend = platformContext.RenderingBackend;
            inputBackend = platformContext.InputBackend;
            

            this.sceneFactory = sceneFactory;
            this.subSystemFactory = subSystemFactory;
        }

        public void Initialize()
        {
            renderingBackend.FontFactory.RegisterFamily("Roboto", Path.Combine(pathProvider.AssetFolderDirectory, "Roboto-VariableFont_wdth,wght.ttf"));

            testScene = sceneFactory.CreateEmptyScene();

            testScene.SystemManager.AddSubSystem<AudioManagementSystem>();

            testScene.SystemManager.AddSubSystem<PhysicsSystem>();

            testScene.SystemManager.AddSubSystem<WorldRenderingSystem>();

            testScene.SystemManager.AddSubSystem<PhysicsInterpolator>();

            gameObject = testScene.CreateGameObject();

            gameObject.Transform.LocalPosition = new Vector2(0f, 0f);

            wallOne = testScene.CreateGameObject();
            wallTwo = testScene.CreateGameObject();
            GameObject wallThree = testScene.CreateGameObject();

            wallOne.Transform.LocalPosition += new Vector2(0, 0);
            wallTwo.Transform.LocalPosition += new Vector2(5, 0);
            wallThree.Transform.LocalPosition += new Vector2(-5, 0);

            cameraObject = testScene.CreateGameObject();

            SpriteRenderer sprite = gameObject.AddComponent<SpriteRenderer>();
            SpriteRenderer wones = wallOne.AddComponent<SpriteRenderer>();
            SpriteRenderer wtwos = wallTwo.AddComponent<SpriteRenderer>();
            SpriteRenderer wthrees = wallThree.AddComponent<SpriteRenderer>();

            //rb2 = wallTwo.AddComponent<RigidBody>();

            rb = gameObject.AddComponent<RigidBody>();

            BoxCollider bc = rb.AddCollider<BoxCollider>();

            rb.PhysicsMode = PhysicsMode.Static;

            rb.GravityScale = 1f;
            
            rb.LockX = false;

            rb.LockY = false;

            rb.FixedRotation = false;

            rb.UseInterpolation = true;

            int objectCount = 2000;

            visuals = new SpriteRenderer[objectCount];
            bodies = new RigidBody[objectCount];

            for (int i = 0; i < objectCount; i++)
            {
                GameObject gameObjectArrayUnit = testScene.CreateGameObject();

                
                if(random.NextDouble() < 0.5f)
                {
                    gameObjectArrayUnit.Transform.LocalPosition = new Vector2((((float)random.NextDouble() * 100) - 50) + 500, (((float)random.NextDouble() * 100) - 50));
                } else
                {
                    gameObjectArrayUnit.Transform.LocalPosition = new Vector2((((float)random.NextDouble() * 100) - 50) - 500, (((float)random.NextDouble() * 100) - 50));
                }

                SpriteRenderer spriteUnit = gameObjectArrayUnit.AddComponent<SpriteRenderer>();

                visuals[i] = spriteUnit;

                spriteUnit.VisualResource.Texture = renderingBackend.TextureFactory.CreateSolidColor(2, 2, Color.White);

                spriteUnit.VisualResource.PixelPerWorldUnit = 1;

                RigidBody rbu = gameObjectArrayUnit.AddComponent<RigidBody>();

                bodies[i] = rbu;

                BoxCollider bcu = rbu.AddCollider<BoxCollider>();

                bcu.Mass = 0.01f;
                bcu.Width = 0.5f;
                bcu.Height = 0.5f;
            }

            //if (bc == null)
            //throw new InvalidOperationException("Failed to create BoxCollider.");

            wallOne.AddComponent<AudioSource>().TryCreateSoundPlayer(Path.Combine(pathProvider.AssetFolderDirectory, "bloop.wav"), out SoundPlayer soundplayer);

            TestSound = soundplayer;

            TestSound.IsLooping = false;

            camera = cameraObject.AddComponent<Camera2D>();

            cameraObject.AddComponent<AudioListener>().IsEnabled = true;

            camera.BlendMode = Rendering.Model.BlendMode.Alpha;

            camera.SamplerMode = Rendering.Model.SamplerMode.Point;

            sprite.VisualResource.Texture = renderingBackend.TextureFactory.Load(Path.Combine(pathProvider.AssetFolderDirectory, "Rat.png"));
            wones.VisualResource.Texture = renderingBackend.TextureFactory.CreateSolidColor(1, 1, Color.Green);
            wtwos.VisualResource.Texture = renderingBackend.TextureFactory.CreateSolidColor(1, 10, Color.Black);
            wthrees.VisualResource.Texture = renderingBackend.TextureFactory.CreateSolidColor(10, 1, Color.White);

            wones.VisualResource.PixelPerWorldUnit = 1;

            wtwos.VisualResource.PixelPerWorldUnit = 3;

            wthrees.VisualResource.PixelPerWorldUnit = 5;

            sprite.VisualResource.PixelPerWorldUnit = 32;

            sprite.ZHeight = 1;

            gamepad = inputBackend.GamepadSlots[0];

            keyboard = inputBackend.KeyboardDevice;

            mouse = inputBackend.MouseDevice;

            IsInitialized = true;
        }

        public void Pause()
        {
            if (!IsInitialized)
                return;
        }

        public void Start()
        {
            if (!IsInitialized)
                return;
        }

        public void Update(FrameTime time)
        {
            if (!IsInitialized)
                return;

            inputBackend.UpdateBackend();

            Vector3 colorFinal = Vector3.Zero;

            for (int i = 0; i < visuals.Length; i++)
            {
                float scaleMax = 5f;

                Vector3 colorA = new Vector3(0, 0, 255);
                Vector3 colorB = new Vector3(0, 255, 0);
                Vector3 colorC = new Vector3(255, 0, 0);

                Vector3 ColorSetA;
                Vector3 ColorSetB;

                float t = float.Clamp(
                    bodies[i].linearVelocity.Length() / scaleMax,
                    0f,
                    1f
                );

                if(t < 0.5)
                {
                    ColorSetA = colorA;
                    ColorSetB = colorB;

                    t *= 2f;
                } 
                else
                {
                    ColorSetA = colorB;
                    ColorSetB = colorC;

                    t = (t - 0.5f) * 2f;
                }

                colorFinal = Vector3.Lerp(ColorSetA, ColorSetB, t);

                Color color = new Color(
                    (byte)colorFinal.X,
                    (byte)colorFinal.Y,
                    (byte)colorFinal.Z
                );

                visuals[i].Color = color;

                bodies[i].AddImpulse((-bodies[i].CurrentPose.Position * 0.005f * time.Delta) + new Vector2((float)(random.NextDouble() - 0.5) * 2f * time.Delta, (float)(random.NextDouble() - 0.5)) * 2f * time.Delta);
            }

            gameObject.Transform.LocalPosition = rb.CurrentPose.Position;

            //wallTwo.Transform.LocalPosition = rb.CurrentPose.Position;

            Vector2 moveAxisKey = new Vector2();

            Vector2 greenMoveAxis = new Vector2();

            if (keyboard.GetKeyDown(KeyCode.Space))
            {
                TestSound.PlaySound();
                TestSound.Pitch = (random.NextSingle() + 0.2f) * 2;
            }

            if (keyboard.GetKey(KeyCode.V))
            {
                wallOne.AddComponent<AudioSource>();
            }

            if (keyboard.GetKey(KeyCode.A))
            {
                moveAxisKey.X = -1;
            }

            if (keyboard.GetKey(KeyCode.D))
            {
                moveAxisKey.X = 1;
            }

            if (keyboard.GetKey(KeyCode.W))
            {
                moveAxisKey.Y = 1;
            }

            if (keyboard.GetKey(KeyCode.S))
            {
                moveAxisKey.Y = -1;
            }

            if (keyboard.GetKey(KeyCode.Left))
            {
                greenMoveAxis.X = -1;
            }

            if (keyboard.GetKey(KeyCode.Right))
            {
                greenMoveAxis.X = 1;
            }

            if (keyboard.GetKey(KeyCode.Up))
            {
                greenMoveAxis.Y = 1;
            }

            if (keyboard.GetKey(KeyCode.Down))
            {
                greenMoveAxis.Y = -1;
            }

            camera.Zoom += mouse.WheelDelta * 0.2f * time.Delta * camera.Zoom;

            float simspeedDelta = 0f;

            if (keyboard.GetKey(KeyCode.T))
            {
                simspeedDelta = 10f * time.Delta;
            }
            else if (keyboard.GetKey(KeyCode.G))
            {
                simspeedDelta = -10f * time.Delta;
            }

            EngineConfig.SimSpeed += simspeedDelta;


            //gameObject.Transform.LocalPosition += moveAxisKey * moveSpeed * time.Delta;

            rb.AddImpulse(moveAxisKey * moveSpeed * time.Delta * 20f);

            wallOne.Transform.LocalPosition += greenMoveAxis * moveSpeed * time.Delta;

            //cameraObject.Transform.LocalPosition = gameObject.Transform.LocalPosition;

            /*Vector2 atb = gameObject.Transform.LocalPosition - cameraObject.Transform.LocalPosition;

            if (atb != Vector2.Zero)
            {
                cameraObject.Transform.LocalPosition += Vector2.Normalize(atb) * cameraSmooth * time.Delta * Vector2.Distance(gameObject.Transform.LocalPosition, cameraObject.Transform.LocalPosition);
            }
            */

            wallOne.Transform.LocalPosition += gamepad.RightThumbstick * moveSpeed * time.Delta;

            wallTwo.Transform.LocalRotation.RotateRadian(gamepad.RightTrigger * time.Delta * moveSpeed);

            wallTwo.Transform.LocalRotation.RotateRadian(-gamepad.LeftTrigger * moveSpeed * time.Delta);

            timeKeep += time.Delta;
            frames++;

            if (timeKeep >= 1f)
            {
                timeKeep = 0;
                Debug.WriteLine($"FPS: {frames} Speed: {rb.LinearVelocity}, Color: r{colorFinal.X} g{colorFinal.Y} b {colorFinal.Z}");
                frames = 0;
            }

            testScene.Update(time);
        }

        public void FixedUpdate(FixedTimeStep time)
        {
            if (!IsInitialized)
                return;

            testScene.FixedUpdate(time);
        }

        public void Render(FrameTime time)
        {
            if (!IsInitialized)
                return;

            renderingBackend.BeginFrame(EngineConfig.ClearColor);

            testScene.Render(time);

            renderingBackend.EndFrame();
        }
    }
}
