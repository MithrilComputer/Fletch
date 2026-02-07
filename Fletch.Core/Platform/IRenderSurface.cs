namespace Fletch.Core.Platform
{
    /// <summary>
    /// Represents a surface that rendering can present into.
    /// </summary>
    public interface IRenderSurface
    {
        /// <summary>
        /// Current width of the render surface in pixels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Current height of the render surface in pixels.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Indicates whether the surface is currently valid for rendering.
        /// </summary>
        public bool IsValid { get; }
    }
}
