using Fletch.Core.Math.Geometry;

namespace Fletch.Core.EngineConfig
{
    internal static class EngineConfig
    {
        public static Vector2Int VirtualResolution { get; } = new Vector2Int(1280, 720);

        public static int TemporaryImportPPU { get; } = 10; //TODO MAKE THIS USER CHANGEABLE

        public static float WorldPixelsPerUnit { get; } = 128;
    }
}
