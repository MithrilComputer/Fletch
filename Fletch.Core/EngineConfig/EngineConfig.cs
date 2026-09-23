using Fletch.Core.Colors;
using Fletch.Core.Math.Geometry;
using System.Numerics;

namespace Fletch.Core.EngineConfig
{
    internal static class EngineConfig
    {
        public static Vector2Int VirtualResolution { get; } = new Vector2Int(1280, 720);

        public static float WorldUnitsAcrossScreen { get; } = 20;

        public static Color ClearColor { get; } = Color.Black;

        public static int AudioSourcePoolSize { get; } = 128;

        public static int PhysicsSubStepCount { get; } = 4;

        public static Vector2 Gravity { get; } = new Vector2(0f, 0.0f);

        public static float SimSpeed { get; set; } = 1f; //TODO Temp
    }
}
