using Fletch.Engine.Components;

namespace Fletch.Engine.Hierarchy
{
    public class GameObject
    {
        public string Name { get; set; } = "GameObject";

        public uint ID { get; internal set; }

        public Transform Transform { get; }

        private List<Component> Components { get; } = new List<Component>();

        public GameObject()
        {
            Transform = new Transform();
        }

        public T? GetComponent<T>() where T : Component
        {
            if (Components.Count == 0)
            {
                return null;
            }

            foreach (Component component in Components)
            {
                if(component is T typedComponent)
                {
                    return typedComponent;
                }
            }

            return null;
        }

        public List<T> GetComponents<T>() where T : Component
        {
            if (Components.Count == 0)
            {
                return new List<T>();
            }

            List<T> typedComponents = new List<T>();

            foreach (Component component in Components)
            {
                if (component is T typedComponent)
                {
                    typedComponents.Add(typedComponent);
                }
            }

            return typedComponents;
        }

        public bool TryAddComponent(Component component)
        {
            if (component == null)
                return false;

            var type = component.GetType();

            if (Components.Any(c => c.GetType() == type))
                return false;

            Components.Add(component);

            component.OnAdded(this);

            return true;
        }

        public bool TryRemoveComponent(Component component)
        {
            if (component == null)
                return false;

            if (!Components.Contains(component))
                return false;

            component.OnRemoved();

            Components.Remove(component);

            return true;
        }
    }
}
