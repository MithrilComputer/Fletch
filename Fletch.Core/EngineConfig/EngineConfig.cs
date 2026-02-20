using Fletch.Core.Colors;
using Fletch.Core.Math.Geometry;

namespace Fletch.Core.EngineConfig
{
    internal static class EngineConfig
    {
        public static Vector2Int VirtualResolution { get; } = new Vector2Int(1280, 720);

        public static float WorldUnitsAcrossScreen { get; } = 20;

        public static Color ClearColor { get; } = Color.CornflowerBlue;

        public static int AudioSourcePoolSize { get; } = 128;
    }
}
