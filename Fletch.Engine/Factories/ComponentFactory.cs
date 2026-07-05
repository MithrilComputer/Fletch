using Fletch.Engine.Abstractions.Factories;
using Fletch.Engine.Components;
using System.Reflection;

namespace Fletch.Engine.Factories
{
    internal class ComponentFactory : IComponentFactory // TODO Vibe coded, refine later, needed time
    {
        private readonly IServiceProvider serviceProvider;

        public ComponentFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public GameObjectComponent CreateNewComponent<T>() where T : GameObjectComponent
        {
            Type componentType = typeof(T);

            ConstructorInfo[] constructors = componentType
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .OrderByDescending(constructorInfo => constructorInfo.GetParameters().Length)
                .ToArray();

            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();

                object?[] arguments = new object?[parameters.Length];

                bool canUseConstructor = true;

                for (int index = 0; index < parameters.Length; index++)
                {
                    ParameterInfo parameter = parameters[index];

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
    }
}