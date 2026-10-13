using Fletch.Core.Colors;
using Fletch.Rendering.Abstractions.Resources;

namespace Fletch.Rendering.Abstractions.Factories
{
    public interface ITextureFactory
    {
        ITexture Load(string path);

        ITexture CreateSolidColor(int width, int height, Color color);

        ITexture SinglePixelTexture { get; }
    }
}
