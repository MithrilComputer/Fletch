using Fletch.Core.Diagnostics;
using Fletch.Engine.Abstractions.Factories;
using Fletch.Engine.Attributes;
using Fletch.Engine.Components;
using Fletch.Engine.Model;
using System.Diagnostics;

namespace Fletch.Engine.Hierarchy
{
    /// <summary>
    /// Represents an entity in a scene, containing a transform and a collection of components.
    /// </summary>
    public sealed class GameObject : IDisposable
    {
        /// <summary>
        /// Gets or sets the name of the GameObject.
        /// </summary>
        public string Name { get; set; } = "GameObject";

        /// <summary>
        /// Gets the unique identifier of the GameObject.
        /// </summary>
        public uint ID { get; internal set; }

        /// <summary>
        /// Gets the GameObject's transform.
        /// </summary>
        public Transform Transform { get; } = new Transform();

        private bool disposed = false;

        private readonly IComponentFactory componentFactory;

        // TODO At some point convert this into a Dictionary<typeof(Component), list<Component>> for faster lookups
        private readonly List<GameObjectComponent> components = new List<GameObjectComponent>();

        private readonly IFletchContextLogger<GameObject> logger;

        public delegate void ComponentChangedHandler(
            GameObjectComponent component,
            ComponentChangeType changeType
        );

        public event ComponentChangedHandler? ComponentChanged;

        internal GameObject(uint id, IFletchContextLogger<GameObject> logger, IComponentFactory componentFactory)
        {
            this.logger = logger;

            this.componentFactory = componentFactory;

            ID = id;

            Transform = new Transform();
        }

        /// <summary>
        /// Creates and adds a component of the specified type.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>The added or existing component.</returns>
        public T? AddComponent<T>() where T : GameObjectComponent
        {
            Type type = typeof(T);
            
            bool disallowMultiple = Attribute.IsDefined(type, typeof(DisallowMultipleComponentAttribute));

            if (disallowMultiple && components.Any(c => c.GetType() == type))
            {
                logger.LogWarning($"{type.Name} does not allow multiple instances.");
                return GetComponent<T>();
            }
            
            GameObjectComponent component = componentFactory.CreateNewComponent<T>(this);

            components.Add(component);

            ComponentChanged?.Invoke(component, ComponentChangeType.Added);

            return (T)component;
        }

        /// <summary>
        /// Gets the first component of the specified type.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>The component, or null if none exists.</returns>
        public T? GetComponent<T>() where T : GameObjectComponent
        {
            if (components.Count == 0)
            {
                return null;
            }

            foreach (GameObjectComponent component in components)
            {
                if (component is T typedComponent)
                {
                    return typedComponent;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all components of the specified type.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>A list of matching components.</returns>
        public List<T> GetComponents<T>() where T : GameObjectComponent
        {
            if (components.Count == 0)
            {
                return new List<T>();
            }

            List<T> typedComponents = new List<T>();

            foreach (GameObjectComponent component in components)
            {
                if (component is T typedComponent)
                {
                    typedComponents.Add(typedComponent);
                }
            }

            return typedComponents;
        }


        /// <summary>
        /// Attempts to remove a component from the GameObject.
        /// </summary>
        /// <param name="component">The component to remove.</param>
        /// <returns>True if removed; otherwise, false.</returns>
        public bool TryRemoveComponent(GameObjectComponent component)
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

        /// <summary>
        /// Enables a component owned by this GameObject.
        /// </summary>
        /// <param name="component">The component to enable.</param>
        public void EnableComponent(GameObjectComponent component)
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

            component.ChangeEnableState(true);

            ComponentChanged?.Invoke(component, ComponentChangeType.Enabled);
        }

        /// <summary>
        /// Disables a component owned by this GameObject.
        /// </summary>
        /// <param name="component">The component to disable.</param>
        public void DisableComponent(GameObjectComponent component)
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

            component.ChangeEnableState(false);

            ComponentChanged?.Invoke(component, ComponentChangeType.Disabled);
        }

        internal void NotifyComponentChanged(GameObjectComponent component, ComponentChangeType changeType)
        {
            if (component == null)
            {
                logger.LogWarning($"Can not notify a system about a Null Component. {Name}, {ID}, {component?.GetType().Name}");

                return;
            }

            if (!components.Contains(component))
            {
                logger.LogWarning($"Can not notify a system about a component that the GameObject does not own. {Name}, {ID}, {component.GetType().Name}");

                return;
            }

            ComponentChanged?.Invoke(component, changeType);
        }

        private void RequestComponentsDestroy()
        {
            foreach (GameObjectComponent component in components.ToArray())
            {
                if (!TryRemoveComponent(component))
                {
                    logger.LogWarning($"Unable to remove component on GameObject destruction. GameObject: {Name}, {ID} Component: {component.GetType().Name}");
                }
            }
        }


        /// <summary>
        /// Removes all components and releases their resources.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            RequestComponentsDestroy();

            components.Clear();

            ComponentChanged = null;

            GC.SuppressFinalize(this);
        }
    }
}
