using Fletch.Platform.Abstractions.Lifecycle;

namespace Fletch.Hosting.API
{
    public sealed class ApplicationLifetime
    {
        private readonly IApplicationLifetime lifetime;

        internal ApplicationLifetime(IApplicationLifetime lifetime)
        {
            this.lifetime = lifetime;
        }

        public bool IsExitRequested => lifetime.IsExitRequested;

        public void RequestExit() => lifetime.RequestExit();

        public event Action? Exiting
        {
            add => lifetime.Exiting += value;
            remove => lifetime.Exiting -= value;
        }
    }
}
