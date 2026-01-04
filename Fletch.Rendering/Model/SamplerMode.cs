namespace Fletch.Rendering.Model
{
    /// <summary>
    /// The sampling mode used when rendering textures.
    /// </summary>
    public enum SamplerMode
    {
        /// <summary>
        /// Linear filtering mode, which smooths textures when scaled.
        /// </summary>
        Linear,

        /// <summary>
        /// Point (nearest neighbor) filtering mode, which preserves hard edges when scaling.
        /// </summary>
        Point
    }
}
