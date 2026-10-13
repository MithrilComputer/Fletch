using Fletch.Engine.Abstractions.Factories;

namespace Fletch.Engine.Systems.Scenes
{
    public class SceneManager
    {
        public Scene? ActiveScene { get; private set; }

        private readonly ISceneFactory sceneFactory;

        internal SceneManager(ISceneFactory sceneFactory)
        {
            this.sceneFactory = sceneFactory;
        }

        public Scene CreateNewScene()
        {
            return sceneFactory.CreateEmptyScene();
        }

        public void SetActiveScene(Scene newScene)
        {
            //TODO
            /*
            // Unload the current scene if it exists
            if (activeScene != null)
            {
                activeScene.Unload();
            }
            // Set the new scene as the active scene
            activeScene = newScene;
            // Load the new scene
            activeScene.Load();
            */

            ActiveScene = newScene;
        }
    }
}
