using Fletch.Rendering.Abstractions.Cameras;
using Fletch.Rendering.Components;

namespace Fletch.Rendering.Abstractions.Managers
{
    internal interface ICameraManager
    {
        IReadOnlyList<Camera2D> Cameras { get; }

        void QueueCreate(Camera2D cameraComponent);

        void QueueRemove(Camera2D cameraComponent);

        ICamera? GetBackendCameraFromBinding(Camera2D camera);

        void FlushSafePoint();

        void MarkDirty();
    }
}
