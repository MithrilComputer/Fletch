using Fletch.Rendering.Components;

namespace Fletch.Rendering.Abstractions.Managers
{
    internal interface ICameraManager
    {
        IReadOnlyList<Camera2D> Cameras { get; }

        void MarkCameraCreation(Camera2D cameraComponent);

        void MarkCameraRemoval(Camera2D cameraComponent);

        void FlushSafePoint();
    }
}
