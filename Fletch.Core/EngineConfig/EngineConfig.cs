using Fletch.Core.Colors;
using Fletch.Core.Math.Geometry;
using System.Numerics;

namespace Fletch.Core.EngineConfig
{
    // Most of these should be per scene settings, but for now they are global engine settings.

    public static class EngineConfig
    {
        internal static BackendSelection BackendSelection { get; } = BackendSelection.MonoGame;

        internal static Vector2Int VirtualResolution { get; } = new Vector2Int(16, 9); // Should never change

        internal static float WorldUnitsAcrossScreen { get; } = 20; // Should never change

        public static Color ClearColor { get; } = Color.Black; //TODO Temp, per camera settings

        internal static int AudioSourcePoolSize { get; } = 128; // Should never change

        public static int PhysicsSubStepCount { get; } = 4; // Temp, per scene setting

        public static Vector2 Gravity { get; } = new Vector2(0f, 0f); //TODO Temp, Per scene setting

        public static float SimSpeed { get; set; } = 1f; //TODO Temp
    }
}
