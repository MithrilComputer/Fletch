using Fletch.Core.Diagnostics;
using Fletch.Engine.Abstractions.Factories;
using Fletch.Engine.Hierarchy.Objects;
using Fletch.Engine.Systems.Scenes;

namespace Fletch.Engine.Factories
{
    internal class GameObjectFactory : ISceneObjectFactory
    {
        private readonly IFletchContextLogger<GameObject> gameObjectLogger;

        private readonly IFletchContextLogger<UIObject> uiObjectLogger;
        private readonly IComponentFactory componentFactory;

        public GameObjectFactory(IFletchContextLogger<GameObject> gameObjectLogger, IFletchContextLogger<UIObject> uiObjectLogger, IComponentFactory componentFactory)
        {
            this.gameObjectLogger = gameObjectLogger;
            this.uiObjectLogger = uiObjectLogger;
            this.componentFactory = componentFactory;
        }

        public GameObject BuildGameObject(uint gameObjectID, Scene host)
        {
            return new GameObject(gameObjectID, gameObjectLogger, componentFactory, host);
        }

        public UIObject BuildUIObject(uint uiObjectID, Scene host)
        {
            return new UIObject(uiObjectID, uiObjectLogger, componentFactory, host);
        }
    }
}
