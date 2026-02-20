using Fletch.Core.Diagnostics;
using Fletch.Engine.Abstractions.Factories;
using Fletch.Engine.Attributes;
using Fletch.Engine.Components;
using Fletch.Engine.Model;

namespace Fletch.Engine.Hierarchy
{
    public sealed class GameObject : IDisposable
    {
        public string Name { get; set; } = "GameObject";

        public uint ID { get; internal set; }

        public Transform Transform { get; }

        // TODO At some point convert this into a Dictionary<typeof(Component), list<Component>> for faster lookups
        private readonly List<Component> components = new List<Component>();

        private readonly IFletchContextLogger<GameObject> logger;

        public delegate void ComponentChangedHandler(
            Component component,
            ComponentChangeType changeType
        );

        public event ComponentChangedHandler? ComponentChanged;

        private readonly IComponentFactory componentFactory;

        internal GameObject(uint id, IFletchContextLogger<GameObject> logger, IComponentFactory componentFactory)
        {
            this.logger = logger;

            this.componentFactory = componentFactory;

            ID = id;

            Transform = new Transform();
        }

        public T? AddComponent<T>() where T : Component
        {
            Type type = typeof(T);

            bool disallowMultiple = Attribute.IsDefined(type, typeof(DisallowMultipleComponentAttribute));

            if (disallowMultiple && components.Any(c => c.GetType() == type))
            {
                logger.LogWarning($"{type.Name} does not allow multiple instances.");
                return GetComponent<T>();
            }

            Component component = componentFactory.CreateNewComponent<T>();

            components.Add(component);

            component.OnAdded(this);

            ComponentChanged?.Invoke(component, ComponentChangeType.Added);

            return (T)component;
        }

        public T? GetComponent<T>() where T : Component
        {
            if (components.Count == 0)
            {
                return null;
            }

            foreach (Component component in components)
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
            if (components.Count == 0)
            {
                return new List<T>();
            }

            List<T> typedComponents = new List<T>();

            foreach (Component component in components)
            {
                if (component is T typedComponent)
                {
                    typedComponents.Add(typedComponent);
                }
            }

            return typedComponents;
        }

        public bool TryRemoveComponent(Component component)
        {
            if (component == null)
                return false;

            if (!components.Contains(component))
                return false;

            component.OnRemoved();

            components.Remove(component);

            ComponentChanged?.Invoke(component, ComponentChangeType.Removed);

            return true;
        }

        public void EnableComponent(Component component)
        {
            if (component == null)
            {
                logger.LogWarning($"Can't Enable a Null Component. {Name}, {ID}, {component?.GetType().Name}");

                return;
            }

            if (!components.Contains(component))
            {
                logger.LogWarning($"Can't Enable a Component that the GameObject does not own. {Name}, {ID}, {component.GetType().Name}");

                return;
            }

            if (component.IsEnabled)
                return;

            component.IsEnabled = true;

            ComponentChanged?.Invoke(component, ComponentChangeType.Enabled);
        }

        public void DisableComponent(Component component)
        {
            if (component == null)
            {
                logger.LogWarning($"Cant Disable a Null Component. {Name}, {ID}, {component?.GetType().Name}");

                return;
            }

            if (!components.Contains(component))
            {
                logger.LogWarning($"Cant Disable a Component that the GameObject does not own. {Name}, {ID}, {component.GetType().Name}");

                return;
            }

            if (!component.IsEnabled)
                return;

            component.IsEnabled = false;

            ComponentChanged?.Invoke(component, ComponentChangeType.Disabled);
        }

        private void RequestComponentsDestroy()
        {
            foreach (Component component in components.ToArray())
            {
                if (!TryRemoveComponent(component))
                {
                    logger.LogWarning($"Unable to remove component on GameObject destruction. GameObject: {Name}, {ID} Component: {component.GetType().Name}");
                }
            }
        }

        public void Dispose()
        {
            RequestComponentsDestroy();
            components.Clear();
        }
    }
}
