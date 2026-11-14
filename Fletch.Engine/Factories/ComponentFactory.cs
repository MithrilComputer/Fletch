using Fletch.Engine.Abstractions.Factories;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy.Objects;
using System.Reflection;

namespace Fletch.Engine.Factories
{
    internal class ComponentFactory : IComponentFactory
    {
        private readonly IServiceProvider serviceProvider;

        public ComponentFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public GameObjectComponent CreateNewGameObjectComponent<T>(GameObject gameObject) where T : GameObjectComponent
        {
            Type componentType = typeof(T);

            ConstructorInfo[] constructors = GameConstructorCache<T>.Constructors;

            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();

                object?[] arguments = new object?[parameters.Length];

                bool canUseConstructor = true;

                for (int index = 0; index < parameters.Length; index++)
                {
                    ParameterInfo parameter = parameters[index];

                    if (parameter.ParameterType == typeof(GameObject))
                    {
                        arguments[index] = gameObject;
                        continue;
                    }

                    object? service = serviceProvider.GetService(parameter.ParameterType);

                    if (service == null && !parameter.HasDefaultValue)
                    {
                        canUseConstructor = false;
                        break;
                    }

                    arguments[index] = service ?? parameter.DefaultValue;
                }

                if (canUseConstructor)
                {
                    object instance = constructor.Invoke(arguments);

                    return (GameObjectComponent)instance;
                }
            }

            throw new InvalidOperationException(
                $"No usable constructor was found for component type '{componentType.FullName}'.");
        }

        public UIComponent CreateNewUIComponent<T>(UIObject gameObject) where T : UIComponent
        {
            Type componentType = typeof(T);

            ConstructorInfo[] constructors = UIConstructorCache<T>.Constructors;

            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();

                object?[] arguments = new object?[parameters.Length];

                bool canUseConstructor = true;

                for (int index = 0; index < parameters.Length; index++)
                {
                    ParameterInfo parameter = parameters[index];

                    if (parameter.ParameterType == typeof(UIObject))
                    {
                        arguments[index] = gameObject;
                        continue;
                    }

                    object? service = serviceProvider.GetService(parameter.ParameterType);

                    if (service == null && !parameter.HasDefaultValue)
                    {
                        canUseConstructor = false;
                        break;
                    }

                    arguments[index] = service ?? parameter.DefaultValue;
                }

                if (canUseConstructor)
                {
                    object instance = constructor.Invoke(arguments);

                    return (UIComponent)instance;
                }
            }

            throw new InvalidOperationException(
                $"No usable constructor was found for component type '{componentType.FullName}'.");
        }

        private static class GameConstructorCache<T> where T : GameObjectComponent
        {
            public static readonly ConstructorInfo[] Constructors =
                typeof(T)
                    .GetConstructors(
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic)
                    .OrderByDescending(c => c.GetParameters().Length)
                    .ToArray();
        }

        private static class UIConstructorCache<T> where T : UIComponent
        {
            public static readonly ConstructorInfo[] Constructors =
                typeof(T)
                    .GetConstructors(
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic)
                    .OrderByDescending(c => c.GetParameters().Length)
                    .ToArray();
        }
    }
}