using Fletch.Core.Components.Update;
using Fletch.Core.LifeCycle;
using Fletch.Core.Time;
using Fletch.Engine.Components.Updateable;
using Fletch.Engine.Hierarchy;

namespace Fletch.Engine.Components
{
    public abstract class Script : GameObjectComponent, IUpdateable, IFixedUpdateable, IStartable, ILateStartable
    {
        public Script(GameObject gameObject) : base(gameObject) { }

        public bool HasStarted { get; set; } = false;

        public bool HasLateStarted { get; set; } = false;

        public virtual void OnStart() { }

        public virtual void OnLateStart() { }

        public virtual void Update(FrameTime deltaTime) { }

        public virtual void FixedUpdate(FixedTimeStep deltaTime) { }
    }
}
