using Fletch.Engine.Hierarchy.Objects;
using Fletch.Engine.Hierarchy.Transforms;
using Fletch.Engine.Model;

namespace Fletch.Engine.Components
{
    public abstract class UIComponent : Component
    {
        public UIObject UIObject { get; private set; }

        protected RectTransform Transform => UIObject.RectTransform;

        public UIComponent(UIObject uiObject)
        {
            UIObject = uiObject;
        }

        protected override void NotifyComponentChanged()
        {
            if (UIObject == null)
                return;

            UIObject.NotifyComponentChanged(this, ComponentChangeType.Modified);
        }

        internal virtual void OnRemoved()
        {

        }
    }
}
