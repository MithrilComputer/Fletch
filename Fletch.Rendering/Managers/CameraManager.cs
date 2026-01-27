using Fletch.Core.Math.Geometry;
using Fletch.Engine.Model;
using Fletch.Rendering.Abstractions.Backends;
using Fletch.Rendering.Abstractions.Cameras;
using Fletch.Rendering.Abstractions.Managers;
using Fletch.Rendering.Components;

namespace Fletch.Rendering.Managers
{
    internal class CameraManager : ICameraManager
    {
        public IReadOnlyList<Camera2D> Cameras => frontendCameras.Items;

        private readonly TrackedSet<Camera2D> frontendCameras = new TrackedSet<Camera2D>();

        private readonly Dictionary<Camera2D, ICamera> cameraBindings = new Dictionary<Camera2D, ICamera>();

        private readonly HashSet<Camera2D> pendingBackendRemoval = new HashSet<Camera2D>();

        private readonly HashSet<Camera2D> pendingBackendCreation = new HashSet<Camera2D>();

        private readonly IRenderingBackend renderingBackend;

        public CameraManager(IRenderingBackend renderingBackend)
        {
            this.renderingBackend = renderingBackend ?? throw new ArgumentNullException(nameof(renderingBackend));
        }

        public void MarkCameraCreation(Camera2D cameraComponent)
        {
            if (cameraComponent == null)
                throw new ArgumentNullException(nameof(cameraComponent));

            if (cameraBindings.ContainsKey(cameraComponent))
                return;

            pendingBackendRemoval.Remove(cameraComponent);
            pendingBackendCreation.Add(cameraComponent);
        }

        public void MarkCameraRemoval(Camera2D cameraComponent)
        {
            if (cameraComponent == null)
                throw new ArgumentNullException(nameof(cameraComponent));

            pendingBackendCreation.Remove(cameraComponent); // cancel creation if queued

            if (!cameraBindings.ContainsKey(cameraComponent)) return;

            pendingBackendRemoval.Add(cameraComponent);
        }

        public void FlushSafePoint()
        {
            foreach (var camera in pendingBackendCreation)
            {
                if (cameraBindings.ContainsKey(camera))
                    continue;

                frontendCameras.MarkToAdd(camera);
                cameraBindings[camera] = renderingBackend.CreateCamera();
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
        }
    }
}
