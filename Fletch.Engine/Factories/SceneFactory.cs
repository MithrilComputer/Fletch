using Fletch.Core.Diagnostics;
using Fletch.Engine.Abstractions.Factories;
using Scene = Fletch.Engine.Scenes.Scene;

namespace Fletch.Engine.Factories
{
    internal class SceneFactory : ISceneFactory
    {
        private readonly IFletchContextLogger<Scene> sceneLogger;

        private readonly IGameObjectFactory gameObjectFactory;

        private readonly ISubSystemFactory subSystemFactory;

        public SceneFactory(IGameObjectFactory gameObjectFactory, IFletchContextLogger<Scene> sceneLogger, ISubSystemFactory subSystemFactory)
        {
            this.sceneLogger = sceneLogger ?? throw new ArgumentNullException(nameof(sceneLogger));
            this.gameObjectFactory = gameObjectFactory ?? throw new ArgumentNullException(nameof(gameObjectFactory));
            this.subSystemFactory = subSystemFactory ?? throw new ArgumentNullException(nameof(subSystemFactory));
        }

        public Scene CreateEmptyScene()
        {
            return new Scene(sceneLogger, gameObjectFactory, subSystemFactory);
        }
    }
}

