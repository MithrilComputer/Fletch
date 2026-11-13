using Fletch.Core.LifeCycle;
using Fletch.Core.Time;
using Fletch.Engine.Hierarchy.Objects;

namespace Fletch.Engine.Components
{
    public class UIScript : UIComponent, IUpdateable, IFixedUpdateable, IStartable, ILateStartable
    {
        public UIScript(UIObject uiObject) : base(uiObject) {}

        public bool HasStarted { get; set; } = false;

        public bool HasLateStarted { get; set; } = false;

        public virtual void OnStart() { }

        public virtual void OnLateStart() { }

        public virtual void Update(FrameTime deltaTime) { }

        public virtual void FixedUpdate(FixedTimeStep deltaTime) { }
    }
}
