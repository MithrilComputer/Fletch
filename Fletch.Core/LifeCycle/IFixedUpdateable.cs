using Fletch.Core.Time;

namespace Fletch.Engine.Components.Updateable
{
    internal interface IFixedUpdateable
    {
        void FixedUpdate(FixedTimeStep deltaTime);
    }
}
