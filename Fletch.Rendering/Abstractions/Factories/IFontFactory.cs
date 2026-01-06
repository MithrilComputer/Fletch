using Fletch.Rendering.Abstractions.Resources;

namespace Fletch.Rendering.Abstractions.Factories
{
    internal interface IFontFactory
    {
        IFont GetFont(string fontID);

    }
}
