using Fletch.Core.Math.Geometry;
using Fletch.Input.Abstractions.Backends;
using Fletch.Input.Model;
using Fletch.Platform.Abstractions.Contexts;
using Fletch.Platform.Abstractions.Lifecycle;
using Fletch.Platform.Abstractions.Paths;
using Fletch.Platform.Abstractions.Window;
using Fletch.Rendering.Abstractions.Backends;
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

        private Vector2 worldCenter = Vector2.Zero;

        private Vector2 cameraAxis;
        private Vector2 drawAxis = Vector2.Zero;

        private float movespeed = 200;

        public FletchRuntime(IPlatformContext platformContext)
        {
            window = platformContext.Window;
            applicationLifeTime = platformContext.Lifetime;
            pathProvider = platformContext.PathProvider;
            renderingBackend = platformContext.RenderingBackend;
            inputBackend = platformContext.InputBackend;
        }

        public IPlatformContext Platform => throw new NotImplementedException();

        public bool IsInitialized { get; private set; }

        public void FixedUpdate(FixedTimeStep time)
        {
            //if(!IsInitialized) Throw later
                
        }

        public void Initialize()
        {
            IsInitialized = true;
        }

        public void Pause()
        {

        }

        public void Render(FrameTime time)
        {
            inputBackend.UpdateBackend();

            renderingBackend.BeginFrame(Color.CornflowerBlue);

            renderingBackend.BeginCamera(renderingBackend.MainCamera, new RectangleInt(
                0,
                0,
                window.Width,
                window.Height
                ));

            renderingBackend.DebugRenderer.DrawCircle(worldCenter, 200, 5);

            renderingBackend.DebugRenderer.DrawCross(worldCenter, 20000, 5, Color.Red);

            renderingBackend.DebugRenderer.DrawCrosshair(worldCenter, 20000, 5, Color.Blue);

            renderingBackend.DebugRenderer.DrawRectangle(drawAxis, Vector2.One * 150, 10, Color.Magenta);

            renderingBackend.EndCamera();

            renderingBackend.EndFrame();
        }

        public void Start()
        {
            
        }

        public void Update(FrameTime time)
        {
            if (inputBackend.KeyboardDevice.GetKeyDown(KeyCode.Escape))
            {
                applicationLifeTime.RequestExit();
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
            }

            if (inputBackend.KeyboardDevice.GetKey(KeyCode.Right))
            {
                xAxisTwo++;
            }

            if (inputBackend.KeyboardDevice.GetKey(KeyCode.Up))
            {
                yAxisTwo++;
            }

            if (inputBackend.KeyboardDevice.GetKey(KeyCode.Down))
            {
                yAxisTwo--;
            }

            cameraAxis = new Vector2(xAxis, yAxis) * time.Delta * movespeed;
            drawAxis += new Vector2(xAxisTwo, yAxisTwo) * time.Delta * movespeed;

            renderingBackend.MainCamera.MoveBy(cameraAxis);

        }
    }
}
