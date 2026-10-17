using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy;
using Fletch.Rendering.Components;

namespace Fletch.Sandbox.Scripts
{
    internal class UIComponent : Script
    {
        private SpriteRenderer renderer;

        public UIComponent(GameObject gameObject) : base(gameObject)
        {
        }

        public override void OnStart()
        {
            renderer = GameObject.GetComponent<SpriteRenderer>();
        }

        public override void Update(FrameTime deltaTime)
        {
            if(!GameState.Started && !GameState.Running && !GameState.Ended)
            {
                renderer.ChangeEnableState(true);
            } else
            {
                renderer.ChangeEnableState(false);
            }
        }
    }
}
