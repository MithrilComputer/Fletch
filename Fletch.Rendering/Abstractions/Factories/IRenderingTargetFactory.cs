using Fletch.Rendering.Abstractions.Resources;

namespace Fletch.Rendering.Abstractions.Factories
{
    public interface IRenderingTargetFactory
    {
        IRenderingTarget Create(int width, int height);
    }
}
