using Fletch.Core.Time;

namespace Fletch.Core.LifeCycle
{
    internal interface IRenderable
    {
        void Render(FrameTime frameTime);
    }
}
