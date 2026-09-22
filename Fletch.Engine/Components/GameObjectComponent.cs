using Fletch.Engine.Hierarchy;
using Fletch.Engine.Model;

namespace Fletch.Engine.Components
{
    public abstract class GameObjectComponent
    {
        public bool IsEnabled { get; set; } = true;

        public GameObject GameObject { get; private set; } 

        protected Transform Transform => GameObject.Transform;

        public GameObjectComponent(GameObject gameObject)
        {
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
