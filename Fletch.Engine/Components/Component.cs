using Fletch.Engine.Hierarchy;

namespace Fletch.Engine.Components
{
    public abstract class Component
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

        internal virtual void OnRemoved()
        {
            GameObject = null!;
        }
    }
}
