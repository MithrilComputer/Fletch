using Fletch.Core.EngineConfig;
using Microsoft.Extensions.DependencyInjection;

namespace Fletch.Hosting.Abstractions.Game
{
    internal interface IPlatformLauncher
    {
        void Run<TApplication>(FletchOptions options, Action<IServiceCollection>? configureServices = null) where TApplication : class, IFletchGame;
    }
}
