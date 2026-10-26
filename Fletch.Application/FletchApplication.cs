using Fletch.Core.EngineConfig;
using Fletch.Hosting.Abstractions.Game;
using Fletch.Platform.MonoGame;

namespace Fletch.Application
{
    public static class FletchApplication
    {
        public static void Run<TGame>(FletchOptions? options = null)
            where TGame : class, IFletchGame
        {
            options ??= new FletchOptions();

            IPlatformLauncher launcher;

            switch (EngineConfig.BackendSelection)
            {
                case BackendSelection.MonoGame:
                    launcher = new MonoGameLauncher();
                    break;

                default:
                    throw new NotSupportedException($"The selected backend '{EngineConfig.BackendSelection}' is not supported.");
            }

            launcher.Run<TGame>(options);
        }
    }
}
