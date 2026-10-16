using Fletch.Application;
using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy;
using Fletch.Hosting.Abstractions.Contexts;
using Fletch.Platform.Abstractions.Paths;
using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.API;
using Fletch.Rendering.Components;
using System.IO;

namespace Fletch.Sandbox.Scripts
{
    internal class CharaterAnimation : Script
    {
        float flapTime = 0.09f;

        float timeKeep = 0f;


        private ITexture preFlapTexture;

        private ITexture postFlapTexture;


        private bool hasFlapped = false;


        private SpriteRenderer renderer;


        private readonly RenderingResources render;

        private readonly IPathProvider pathProvider;


        public CharaterAnimation(GameObject gameObject, IGameContext context) : base(gameObject)
        {
            render = context.Rendering;
            pathProvider = context.PathProvider;
        }

        public override void OnStart()
        {
            renderer = GameObject.GetComponent<SpriteRenderer>();

            preFlapTexture = render.TextureFactory.Load(Path.Combine(pathProvider.AssetFolderDirectory, "PlayerTwo.png"));

            postFlapTexture = render.TextureFactory.Load(Path.Combine(pathProvider.AssetFolderDirectory, "PlayerOne.png"));

            renderer.VisualResource.Texture = preFlapTexture;
        }

        public override void Update(FrameTime deltaTime)
        {
            if(hasFlapped)
            {
                timeKeep += deltaTime.Delta;

                if (timeKeep >= flapTime)
                {
                    renderer.VisualResource.Texture = preFlapTexture;
                    hasFlapped = false;
                    timeKeep = 0f;
                }
            }
        }

        public void OnJump()
        {
            if(hasFlapped)
                return;

            renderer.VisualResource.Texture = postFlapTexture;

            hasFlapped = true;
        }
    }
}
