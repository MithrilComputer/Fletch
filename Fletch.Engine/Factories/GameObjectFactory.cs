using Fletch.Core.Diagnostics;
using Fletch.Engine.Abstractions.Factories;
using Fletch.Engine.Hierarchy;

namespace Fletch.Engine.Factories
{
    internal class GameObjectFactory : IGameObjectFactory
    {
        private readonly IFletchContextLogger<GameObject> gameObjectLogger;

        private readonly IComponentFactory componentFactory;

        public GameObjectFactory(IFletchContextLogger<GameObject> gameObjectLogger, IComponentFactory componentFactory)
        {
            this.gameObjectLogger = gameObjectLogger;
            this.componentFactory = componentFactory;
        }

        public GameObject BuildGameObject(uint gameObjectID)
        {
            return new GameObject(gameObjectID, gameObjectLogger, componentFactory);
        }
    }
}
