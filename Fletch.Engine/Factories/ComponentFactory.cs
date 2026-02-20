using Fletch.Engine.Abstractions.Factories;
using Fletch.Engine.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Fletch.Engine.Factories
{
    internal class ComponentFactory : IComponentFactory
    {
        private readonly IServiceProvider serviceProvider;

        public ComponentFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public Component CreateNewComponent<T>() where T : Component
        {
            return ActivatorUtilities.CreateInstance<T>(serviceProvider);
        }
    }
}
