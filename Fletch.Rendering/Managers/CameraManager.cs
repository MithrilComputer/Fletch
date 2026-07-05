using Fletch.Engine.Model;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Rendering.Abstractions.Cameras;
using Fletch.Rendering.Abstractions.Managers;
using Fletch.Rendering.Components;

namespace Fletch.Rendering.Managers
{
    internal sealed class CameraManager : ICameraManager, IDisposable
    {
        public IReadOnlyList<Camera2D> Cameras => frontendCameras.Items;

        private readonly TrackedSet<Camera2D> frontendCameras = new TrackedSet<Camera2D>();

        private readonly Dictionary<Camera2D, ICamera> cameraBindings = new Dictionary<Camera2D, ICamera>();

        private readonly HashSet<Camera2D> pendingBackendRemoval = new HashSet<Camera2D>();

        private readonly HashSet<Camera2D> pendingBackendCreation = new HashSet<Camera2D>();

        private readonly IRenderingBackend renderingBackend;

        private bool cameraOrderDirty = true;

        private int renderOrderIndex = 0;

        public CameraManager(IRenderingBackend renderingBackend)
        {
            this.renderingBackend = renderingBackend ?? throw new ArgumentNullException(nameof(renderingBackend));
        }

        public void MarkDirty()
        {
            cameraOrderDirty = true;
        }

        public void QueueCreate(Camera2D cameraComponent)
        {
            if (cameraComponent == null)
                throw new ArgumentNullException(nameof(cameraComponent));

            if (cameraBindings.ContainsKey(cameraComponent))
                return;

            pendingBackendRemoval.Remove(cameraComponent);
            pendingBackendCreation.Add(cameraComponent);

            cameraOrderDirty = true;
        }

        public void QueueRemove(Camera2D cameraComponent)
        {
            if (cameraComponent == null)
                throw new ArgumentNullException(nameof(cameraComponent));

            pendingBackendCreation.Remove(cameraComponent); // cancel creation if queued

            if (!cameraBindings.ContainsKey(cameraComponent)) return;

            pendingBackendRemoval.Add(cameraComponent);

            cameraOrderDirty = true;
        }

        public void FlushSafePoint()
        {
            foreach (var camera in pendingBackendCreation)
            {
                if (cameraBindings.ContainsKey(camera))
                    continue;

                frontendCameras.MarkToAdd(camera);
                cameraBindings[camera] = renderingBackend.CreateCamera();

                camera.RenderIndex = renderOrderIndex;
                renderOrderIndex++;
            }

            foreach (var camera in pendingBackendRemoval)
            {
                if (!cameraBindings.TryGetValue(camera, out var backendCamera))
                    continue;

                frontendCameras.MarkToRemove(camera);
                renderingBackend.RemoveCamera(backendCamera);
                cameraBindings.Remove(camera);
            }

            pendingBackendCreation.Clear();
            pendingBackendRemoval.Clear();

            frontendCameras.Refresh();

            if (cameraOrderDirty)
            {
                frontendCameras.Sort(SystemCompare);
            }
        }

        public ICamera? GetBackendCameraFromBinding(Camera2D camera)
        {
            cameraBindings.TryGetValue(camera, out ICamera? backendCamera);

            return backendCamera;
        }

        private static int SystemCompare(Camera2D a, Camera2D b)
        {
            int order = a.RenderOrder.CompareTo(b.RenderOrder);
            if (order != 0)
                return order;

            return a.RenderIndex.CompareTo(b.RenderIndex);
        }

        public void Dispose()
        {
            foreach (Camera2D camera in frontendCameras.Items)
            {
                QueueRemove(camera);
            }

            FlushSafePoint();
        }
    }
}
