using Fletch.Core.Math.Geometry;
using Fletch.Input.Abstractions.Backends;
using Fletch.Input.Model;
using Fletch.Platform.Abstractions.Contexts;
using Fletch.Platform.Abstractions.Lifecycle;
using Fletch.Platform.Abstractions.Paths;
using Fletch.Platform.Abstractions.Window;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.Model;
using Fletch.Runtime.Abstractions.Hosting;
using Fletch.Runtime.Abstractions.Time;
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

        public IPlatformContext Platform => throw new NotImplementedException();

        public bool IsInitialized { get; private set; }

        // TESTIN STUFFFFF __________________

        private Vector2 worldCenter = Vector2.Zero;

        private Vector2 cameraAxis;
        private Vector2 drawAxis = Vector2.Zero;

        private float movespeed = 200;

        SpriteEffect testRatFlip = SpriteEffect.None;

        IFont testFont;

        ITexture testTexture;

        float timekeep = 0f;

        string fps = "";

        int frameCountFps = 0;

        float spriteRotation = 0;

        float rotationSpeed = 5;

        float zoom = 1f;

        float zoomSpeed = 1f;

        // TESTIN STUFFFFF __________________

        public FletchRuntime(IPlatformContext platformContext)
        {
            window = platformContext.Window;
            applicationLifeTime = platformContext.Lifetime;
            pathProvider = platformContext.PathProvider;
            renderingBackend = platformContext.RenderingBackend;
            inputBackend = platformContext.InputBackend;
        }

        public void Initialize()
        {
            renderingBackend.FontFactory.RegisterFamily("Roboto", Path.Combine(pathProvider.AssetFolderDirectory, "Roboto-VariableFont.ttf"));

            testFont = renderingBackend.FontFactory.GetFont("Roboto", 64);

            testTexture = renderingBackend.TextureFactory.Load(Path.Combine(pathProvider.AssetFolderDirectory, "Rat.png"));

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

        public void Render(FrameTime time)
        {
            if (!IsInitialized)
                return;

            inputBackend.UpdateBackend();

            renderingBackend.BeginFrame(Color.CornflowerBlue);

            renderingBackend.BeginCamera(renderingBackend.MainCamera, new RectangleInt(
                0,
                0,
                window.Width,
                window.Height
                ));

            renderingBackend.MainCamera.SetZoom(zoom);

            renderingBackend.DebugRenderer.DrawCircle(worldCenter, 200, 5);

            renderingBackend.DebugRenderer.DrawCross(worldCenter, 20000, 5, Color.Red);

            renderingBackend.DebugRenderer.DrawCrosshair(worldCenter, 20000, 5, Color.Blue);

            renderingBackend.DebugRenderer.DrawRectangle(drawAxis, Vector2.One * 150, 10, Color.Magenta);

            renderingBackend.DebugRenderer.DrawLine(drawAxis, Vector2.UnitY * 300, 10, Color.Brown);

            renderingBackend.SpriteBatcher.DrawString(testFont, fps, drawAxis, Color.Black);

            renderingBackend.SpriteBatcher.Draw(testTexture, drawAxis, new RectangleFloat(0,0, testTexture.Width, testTexture.Height), Color.White, spriteRotation, Vector2.Zero, Vector2.One, 0, testRatFlip);

            renderingBackend.EndCamera();

            renderingBackend.EndFrame();
        }

        public void Update(FrameTime time)
        {
            if (!IsInitialized)
                return;

            if (inputBackend.KeyboardDevice.GetKeyDown(KeyCode.Escape))
            {
                applicationLifeTime.RequestExit();
            }

            timekeep += time.Delta;

            frameCountFps++;

            if (timekeep >= 1)
            {
                Debug.WriteLine(frameCountFps);

                fps = frameCountFps.ToString();

                frameCountFps = 0;
                timekeep = 0;
            }

            float xAxis = 0f;
            float yAxis = 0f;

            float xAxisTwo = 0f;
            float yAxisTwo = 0f;

            if (inputBackend.KeyboardDevice.GetKeyDown(KeyCode.Escape))
            {
                applicationLifeTime.RequestExit();
            }

            if (inputBackend.KeyboardDevice.GetKey(KeyCode.A))
            {
                xAxis--;
            }

            if (inputBackend.KeyboardDevice.GetKey(KeyCode.D))
            {
                xAxis++;
            }

            if (inputBackend.KeyboardDevice.GetKey(KeyCode.W))
            {
                yAxis++;
            }

            if (inputBackend.KeyboardDevice.GetKey(KeyCode.S))
            {
                yAxis--;
            }

            if (inputBackend.KeyboardDevice.GetKey(KeyCode.Left))
            {
                xAxisTwo--;
                testRatFlip = SpriteEffect.None;
            }

            if (inputBackend.KeyboardDevice.GetKey(KeyCode.Right))
            {
                xAxisTwo++;
                testRatFlip = SpriteEffect.FlipHorizontally;
            }

            if (inputBackend.KeyboardDevice.GetKey(KeyCode.Up))
            {
                yAxisTwo++;
            }

            if (inputBackend.KeyboardDevice.GetKey(KeyCode.Down))
            {
                yAxisTwo--;
            }

            if (inputBackend.KeyboardDevice.GetKey(KeyCode.Q))
            {
                spriteRotation += rotationSpeed * time.Delta;
            }

            if (inputBackend.KeyboardDevice.GetKey(KeyCode.E))
            {
                spriteRotation -= rotationSpeed * time.Delta;
            }

            if (inputBackend.KeyboardDevice.GetKey(KeyCode.Z))
            {
                zoom += zoomSpeed * time.Delta * 0.1f;
            }

            if (inputBackend.KeyboardDevice.GetKey(KeyCode.X))
            {
                zoom -= zoomSpeed * time.Delta * 0.1f;
            }

            cameraAxis = new Vector2(xAxis, yAxis) * time.Delta * movespeed;
            drawAxis += new Vector2(xAxisTwo, yAxisTwo) * time.Delta * movespeed * 2;

            renderingBackend.MainCamera.MoveBy(cameraAxis);
        }

        public void FixedUpdate(FixedTimeStep time)
        {
            if (!IsInitialized)
                return;

        }
    }
}
