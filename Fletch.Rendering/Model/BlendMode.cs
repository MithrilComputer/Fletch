namespace Fletch.Rendering.Model
{
    /// <summary>
    /// The blending modes available when rendering.
    /// </summary>
    public enum BlendMode
    {
        /// <summary>
        /// Alpha blending mode, which blends based on the alpha channel.
        /// </summary>
        Alpha,

        /// <summary>
        /// Additive blending mode, which adds color values together.
        /// </summary>
        Additive,

        /// <summary>
        /// Opaque blending mode, which renders without any transparency.
        /// </summary>
        Opaque
    }
}
