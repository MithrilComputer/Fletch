using Fletch.Core.EngineConfig;

namespace Fletch.Hosting.Abstractions.Game
{
    internal interface IPlatformLauncher
    {
        void Run<TApplication>(FletchOptions options) where TApplication : class, IFletchGame;
    }
}
