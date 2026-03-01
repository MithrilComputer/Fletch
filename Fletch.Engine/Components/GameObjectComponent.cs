using Fletch.Engine.Hierarchy;
using Fletch.Engine.Model;

namespace Fletch.Engine.Components
{
    public abstract class GameObjectComponent
    {
        public bool IsEnabled { get; set; } = true;

        public GameObject GameObject { get; private set; } = null!;

        protected Transform Transform => GameObject?.Transform ?? throw new InvalidOperationException("Component is not attached to a GameObject.");

        internal virtual void OnAdded(GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            if (GameObject != null)
                throw new InvalidOperationException(
                    $"{GetType().Name} is already attached to a GameObject.");

            GameObject = gameObject;
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
