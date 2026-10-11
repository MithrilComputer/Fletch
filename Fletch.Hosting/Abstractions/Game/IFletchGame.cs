using Fletch.Hosting.Abstractions.Contexts;

namespace Fletch.Hosting.Abstractions.Game
{
    public interface IFletchGame
    {
        void Initialize(IGameContext context);

        void Start();

        void Shutdown();
    }
}
