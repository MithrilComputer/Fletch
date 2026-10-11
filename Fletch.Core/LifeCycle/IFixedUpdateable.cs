using Fletch.Core.Time;

namespace Fletch.Core.LifeCycle
{
    internal interface IFixedUpdateable
    {
        void FixedUpdate(FixedTimeStep deltaTime);
    }
}
