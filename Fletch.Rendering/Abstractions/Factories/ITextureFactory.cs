using Fletch.Core.Colors;
using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.Model;

namespace Fletch.Rendering.Abstractions.Factories
{
    internal interface ITextureFactory
    {
        ITexture Load(string path);

        ITexture CreateSolidColor(int width, int height, Color color);

        ITexture SinglePixelTexture { get; }
    }
}
