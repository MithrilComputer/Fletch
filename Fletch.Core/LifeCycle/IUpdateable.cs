using Fletch.Core.Time;

namespace Fletch.Core.Components.Update
{
    internal interface IUpdateable
    {
        void Update(FrameTime deltaTime);
    }
}
