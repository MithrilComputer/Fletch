using Microsoft.Extensions.DependencyInjection;

namespace Fletch.Platform.MonoGame.Contexts
{
    internal sealed class MonoGameBootContext : IDisposable
    {
        private readonly ServiceProvider serviceProvider;

        public Action<float> FixedUpdate { get; }

        public Action<float> Update { get; }

        public Action Render { get; }

        public MonoGameBootContext(
            ServiceProvider serviceProvider,
            Action<float> fixedUpdate,
            Action<float> update,
            Action render)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            FixedUpdate = fixedUpdate ?? throw new ArgumentNullException(nameof(fixedUpdate));
            Update = update ?? throw new ArgumentNullException(nameof(update));
            Render = render ?? throw new ArgumentNullException(nameof(render));
        }

        public void Dispose()
        {
            serviceProvider.Dispose();
        }
    }
}
