using Fletch.Hosting.Abstractions.Contexts;
using Fletch.Hosting.Abstractions.Game;
using System.IO;

namespace Fletch.Sandbox
{
    internal class TestGame : IFletchGame
    {
        private LevelOne levelOne;

        public void Initialize(IGameContext context)
        {
            context.Rendering.FontFactory.RegisterFamily("Roboto", Path.Combine(context.PathProvider.AssetFolderDirectory,"Roboto-VariableFont_wdth,wght.ttf"));

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
