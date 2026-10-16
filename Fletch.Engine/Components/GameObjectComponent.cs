using Fletch.Engine.Hierarchy;
using Fletch.Engine.Model;

namespace Fletch.Engine.Components
{
    public abstract class GameObjectComponent
    {
        public bool IsEnabled { get; private set; } = true;

        public event Action<bool>? EnableStateChanged;

        public GameObject GameObject { get; private set; }

        protected Transform Transform => GameObject.Transform;

        public GameObjectComponent(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        public void ChangeEnableState(bool enabled)
        {
            if (IsEnabled == enabled)
                return;

            IsEnabled = enabled;

            EnableStateChanged?.Invoke(enabled);
        }

        protected void NotifyComponentChanged()
        {
            if (GameObject == null)
                return;

            GameObject.NotifyComponentChanged(this, ComponentChangeType.Modified);
        }

        internal virtual void OnRemoved()
        {
            GameObject = null!;
        }
    }
}
