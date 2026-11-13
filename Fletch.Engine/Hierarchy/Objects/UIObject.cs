using Fletch.Core.Diagnostics;
using Fletch.Engine.Abstractions.Factories;
using Fletch.Engine.Attributes;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy.Transforms;
using Fletch.Engine.Model;
using Fletch.Engine.Systems.Scenes;

namespace Fletch.Engine.Hierarchy.Objects
{
    public sealed class UIObject : SceneObject
    {
        public RectTransform RectTransform { get; } = new RectTransform();

        private bool disposed = false;

        private readonly Scene hostScene;

        private readonly IComponentFactory componentFactory;

        private readonly List<UIComponent> components = new List<UIComponent>();

        private readonly IFletchContextLogger<UIObject> logger;

        public delegate void ComponentChangedHandler(
            UIComponent component,
            ComponentChangeType changeType
        );

        public event ComponentChangedHandler? ComponentChanged;

        internal UIObject(uint id, IFletchContextLogger<UIObject> logger, IComponentFactory componentFactory, Scene hostScene) : base(id, "UIObject", hostScene)
        {
            this.logger = logger;

            this.componentFactory = componentFactory;

            this.hostScene = hostScene;

            RectTransform = new RectTransform();
        }

        /// <summary>
        /// Creates and adds a component of the specified type.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>The added or existing component.</returns>
        public T? AddComponent<T>() where T : UIComponent
        {
            Type type = typeof(T);

            bool disallowMultiple = Attribute.IsDefined(type, typeof(DisallowMultipleComponentAttribute));

            if (disallowMultiple && components.Any(c => c.GetType() == type))
            {
                logger.LogWarning($"{type.Name} does not allow multiple instances.");
                return GetComponent<T>();
            }

            UIComponent component = componentFactory.CreateNewUIComponent<T>(this);

            components.Add(component);

            ComponentChanged?.Invoke(component, ComponentChangeType.Added);

            return (T)component;
        }

        /// <summary>
        /// Gets the first component of the specified type.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>The component, or null if none exists.</returns>
        public T? GetComponent<T>() where T : UIComponent
        {
            if (components.Count == 0)
            {
                return null;
            }

            foreach (UIComponent component in components)
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
        public List<T> GetComponents<T>() where T : UIComponent
        {
            if (components.Count == 0)
            {
                return new List<T>();
            }

            List<T> typedComponents = new List<T>();

            foreach (UIComponent component in components)
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
        public bool TryRemoveComponent(UIComponent component)
        {
            if (component == null)
                return false;

            if (!components.Remove(component))
                return false;

            ComponentChanged?.Invoke(component, ComponentChangeType.Removed);

            component.OnRemoved();

            return true;
        }

        /// <summary>
        /// Enables a component owned by this GameObject.
        /// </summary>
        /// <param name="component">The component to enable.</param>
        public void EnableComponent(UIComponent component)
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
        public void DisableComponent(UIComponent component)
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

        internal void NotifyComponentChanged(UIComponent component, ComponentChangeType changeType)
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
            foreach (UIComponent component in components.ToArray())
            {
                if (!TryRemoveComponent(component))
                {
                    logger.LogWarning($"Unable to remove component on GameObject destruction. GameObject: {Name}, {ID} Component: {component.GetType().Name}");
                }
            }
        }

        /// <summary>
        /// Notifies the scene to destroy the GameObject.
        /// </summary>
        public override void Destroy()
        {
            hostScene.MarkObjectForDistruction(this);
        }

        protected override void OnDispose()
        {
            if (disposed)
                return;

            disposed = true;

            RequestComponentsDestroy();
        }
    }
}
