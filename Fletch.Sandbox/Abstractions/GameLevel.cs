using Fletch.Engine.Systems.Scenes;
using Fletch.Hosting.Abstractions.Contexts;
using Fletch.Hosting.API;
using Fletch.Input.API;
using Fletch.Platform.Abstractions.Paths;
using Fletch.Rendering.Abstractions.Drawing;
using Fletch.Rendering.Abstractions.Factories;

namespace Fletch.Sandbox.Abstractions
{
    internal class GameLevel
    {
        protected readonly Scene levelScene;

        protected readonly IGameContext gameContext;

        protected readonly ITextureFactory textureFactory;
        protected readonly IDebugRenderer debugRenderer;
        protected readonly SceneManager sceneManager;
        protected readonly InputService input;
        protected readonly IPathProvider pathProvider;
        protected readonly ApplicationLifetime applicationLifetime;

        public GameLevel(IGameContext gameContext)
        {
            this.gameContext = gameContext;

            sceneManager = gameContext.SceneManager;

            textureFactory = gameContext.Rendering.TextureFactory;
            debugRenderer = gameContext.Rendering.DebugRenderer;
            input = gameContext.Input;
            pathProvider = gameContext.PathProvider;
            applicationLifetime = gameContext.ApplicationLifetime;

            levelScene = gameContext.SceneManager.CreateNewScene();

            sceneManager.SetActiveScene(levelScene);
        }
    }
}
