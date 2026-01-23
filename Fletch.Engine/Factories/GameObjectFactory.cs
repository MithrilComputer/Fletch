using Fletch.Core.Diagnostics;
using Fletch.Engine.Abstractions.Factories;
using Fletch.Engine.Hierarchy;

namespace Fletch.Engine.Factories
{
    internal class GameObjectFactory : IGameObjectFactory
    {
        private readonly IFletchContextLogger<GameObject> gameObjectLogger;

        public GameObjectFactory(IFletchContextLogger<GameObject> gameObjectLogger)
        {
            this.gameObjectLogger = gameObjectLogger;
        }

        public GameObject BuildGameObject(uint gameObjectID)
        {
            return new GameObject(gameObjectID, gameObjectLogger);
        }
    }
}
