namespace Fletch.Rendering.Abstractions.Resources
{
    /// <summary>
    /// A GPU resource that can be rendered into
    /// and also sampled as a texture.
    /// </summary>
    public interface IRenderingTarget : ITexture, IDisposable;
}
