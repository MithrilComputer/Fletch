namespace Fletch.Rendering.Abstractions.Resources
{
    /// <summary>
    /// Shared visual data for 2D rendering.
    /// </summary>
    public class VisualResource
    {
        /// <summary>
        /// A human-readable name for this visual resource.
        /// </summary>
        public string Name { get; set; } = "Unnamed Visual Resource";

        /// <summary>
        /// Texture used for rendering.
        /// </summary>
        public ITexture? Texture { get; set; } = null;

        /// <summary>
        /// Pixels that equal one world unit.
        /// </summary>
        public int PixelPerWorldUnit { get; set; } = 32;
    }
}
