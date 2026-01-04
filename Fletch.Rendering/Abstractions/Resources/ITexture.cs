namespace Fletch.Rendering.Abstractions.Resources
{
    /// <summary>
    /// Represents a GPU texture resource in a backend-agnostic way.
    /// </summary>
    public interface ITexture : IDisposable
    {
        /// <summary>
        /// The width of the texture in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// The height of the texture in pixels.
        /// </summary>
        int Height { get; }
    }
}
