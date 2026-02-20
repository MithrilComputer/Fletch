using Fletch.Core.Colors;
using Fletch.Core.EngineConfig;
using Fletch.Core.Math.Geometry;
using Fletch.Core.Time;
using Fletch.Engine.Abstractions.Factories;
using Fletch.Engine.Hierarchy;
using Fletch.Engine.Scenes;
using Fletch.Input.Abstractions.Backends;
using Fletch.Input.Abstractions.InputDevices;
using Fletch.Platform.Abstractions.Contexts;
using Fletch.Platform.Abstractions.Lifecycle;
using Fletch.Platform.Abstractions.Paths;
using Fletch.Platform.Abstractions.Window;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.Components;
using Fletch.Rendering.Systems;
using Fletch.Runtime.Abstractions.Hosting;
using System.Diagnostics;
using System.Numerics;

namespace Fletch.Runtime.Hosting
{
    internal class FletchRuntime : IRuntime
    {
        private readonly IWindow window;

        private readonly IApplicationLifetime applicationLifeTime;

        private readonly IPathProvider pathProvider;

        private readonly IRenderingBackend renderingBackend;

        private readonly IInputBackend inputBackend;

        private readonly ISceneFactory sceneFactory;

        private readonly ISubSystemFactory subSystemFactory;

        public IPlatformContext Platform => throw new NotImplementedException();

        public bool IsInitialized { get; private set; }

        //Testing

        private Scene testScene;

        private GameObject gameObject;

        GameObject wallOne;

        GameObject wallTwo;

        private GameObject cameraObject;

        private Camera2D camera;

        private IGamepad gamepad;

        private float moveSpeed = 5f;

        private float cameraSmooth = 2f;

        private float timeKeep = 0f;

        private int frames = 0;

        //Testing

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
            renderingBackend.FontFactory.RegisterFamily("Roboto", Path.Combine(pathProvider.AssetFolderDirectory, "Roboto-VariableFont.ttf"));

            testScene = sceneFactory.CreateEmptyScene();

            testScene.SystemManager.AddSubSystem<WorldRenderingSystem>();

            gameObject = testScene.CreateGameObject();

            wallOne = testScene.CreateGameObject();
            wallTwo = testScene.CreateGameObject();
            GameObject wallThree = testScene.CreateGameObject();

            wallOne.Transform.LocalPosition += new Vector2(0, 0);
            wallTwo.Transform.LocalPosition += new Vector2(10, 0);
            wallThree.Transform.LocalPosition += new Vector2(-10, 0);

            cameraObject = testScene.CreateGameObject();

            SpriteRenderer sprite = gameObject.AddComponent<SpriteRenderer>();
            SpriteRenderer wones = wallOne.AddComponent<SpriteRenderer>();
            SpriteRenderer wtwos = wallTwo.AddComponent<SpriteRenderer>();
            SpriteRenderer wthrees = wallThree.AddComponent<SpriteRenderer>();

            camera = cameraObject.AddComponent<Camera2D>();

            camera.BlendMode = Rendering.Model.BlendMode.Alpha;

            /* testing
            for (int i = 0; i < 10000; i++)
            {
                GameObject objjec = testScene.CreateGameObject();
                SpriteRenderer sprited = objjec.AddComponent<SpriteRenderer>();
                sprited.Texture = renderingBackend.TextureFactory.CreateSolidColor(1, 1, Color.Green);
            }
            */

            camera.SamplerMode = Rendering.Model.SamplerMode.Point;

            sprite.VisualResource.Texture = renderingBackend.TextureFactory.Load(Path.Combine(pathProvider.AssetFolderDirectory, "Rat.png"));
            wones.VisualResource.Texture = renderingBackend.TextureFactory.CreateSolidColor(1, 1, Color.Green);
            wtwos.VisualResource.Texture = renderingBackend.TextureFactory.CreateSolidColor(1, 10, Color.Black);
            wthrees.VisualResource.Texture = renderingBackend.TextureFactory.CreateSolidColor(10, 1, Color.White);

            wones.VisualResource.PixelPerWorldUnit = 1;

            wtwos.VisualResource.PixelPerWorldUnit = 3;

            wthrees.VisualResource.PixelPerWorldUnit = 5;

            sprite.ZHeight = 1;

            gamepad = inputBackend.GamepadSlots[0];

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

            gameObject.Transform.LocalPosition += gamepad.LeftThumbstick * moveSpeed * time.Delta;

            Vector2 atb = gameObject.Transform.LocalPosition - cameraObject.Transform.LocalPosition;

            if (atb != Vector2.Zero)
            {
                cameraObject.Transform.LocalPosition += Vector2.Normalize(atb) * cameraSmooth * time.Delta * Vector2.Distance(gameObject.Transform.LocalPosition, cameraObject.Transform.LocalPosition);
            }

            wallOne.Transform.LocalPosition += gamepad.RightThumbstick * moveSpeed * time.Delta;

            wallTwo.Transform.LocalRotation.RotateRadian(gamepad.RightTrigger * time.Delta * moveSpeed);

            wallTwo.Transform.LocalRotation.RotateRadian(-gamepad.LeftTrigger * moveSpeed * time.Delta);

            timeKeep += time.Delta;
            frames++;

            if (timeKeep >= 1)
            {
                timeKeep = 0;
                Debug.WriteLine($"FPS: {frames}");
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
