using Fletch.Core.Components.Update;
using Fletch.Core.LifeCycle;
using Fletch.Engine.Components.Updateable;

namespace Fletch.Engine.Components
{
    public abstract class Script : Component, IUpdateable, IFixedUpdateable, IStartable, ILateStartable
    {
        public virtual void OnStart() { }

        public virtual void OnLateStart() { }

        public virtual void Update(float deltaTime) { }

        public virtual void FixedUpdate(float deltaTime) { }
    }
}
