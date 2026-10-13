using Fletch.Hosting.Abstractions.Contexts;
using Fletch.Hosting.Abstractions.Game;

namespace Fletch.Sandbox
{
    internal class TestGame : IFletchGame
    {
        private LevelOne levelOne;

        public void Initialize(IGameContext context)
        {
            levelOne = new LevelOne(context);
        }

        public void Shutdown()
        {

        }

        public void Start()
        {

        }
    }
}
