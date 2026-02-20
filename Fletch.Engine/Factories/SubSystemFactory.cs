using Fletch.Engine.Abstractions.Factories;
using Fletch.Engine.Systems;
using Microsoft.Extensions.DependencyInjection;

namespace Fletch.Engine.Factories
{
    internal sealed class SubSystemFactory : ISubSystemFactory
    {
        private readonly IServiceProvider serviceProvider;

        public SubSystemFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public T CreateSubSystem<T>() where T : SceneSubsystem
        {
            try
            {
                return ActivatorUtilities.CreateInstance<T>(serviceProvider);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to create subsystem '{typeof(T).FullName}'. " +
                    $"Constructor dependencies were not satisfied. " +
                    $"Inner: {ex.GetBaseException().Message}",
                    ex);
            }
        }
    }
}
