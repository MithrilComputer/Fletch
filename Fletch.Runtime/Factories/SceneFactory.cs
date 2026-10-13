using Fletch.Audio.Systems;
using Fletch.Core.Diagnostics;
using Fletch.Engine.Abstractions.Factories;
using Fletch.Engine.Systems;
using Fletch.Engine.Systems.Scenes;
using Fletch.Physics.Systems;
using Fletch.Rendering.Systems;
using Fletch.Runtime.Systems;

namespace Fletch.Runtime.Factories
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
            Scene scene = new Scene(sceneLogger, gameObjectFactory, subSystemFactory);

            scene.SystemManager.AddSubSystem<ScriptRunnerSystem>();

            scene.SystemManager.AddSubSystem<AudioManagementSystem>();
            scene.SystemManager.AddSubSystem<WorldRenderingSystem>();
            scene.SystemManager.AddSubSystem<PhysicsSystem>();

            scene.SystemManager.AddSubSystem<PhysicsInterpolator>();

            return scene;
        }
    }
}

