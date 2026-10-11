using Fletch.Core.Time;

namespace Fletch.Core.LifeCycle
{
    internal interface IUpdateable
    {
        void Update(FrameTime deltaTime);
    }
}
