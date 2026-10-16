using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy;

namespace Fletch.Sandbox.Scripts
{
    internal class EmptyScript : Script
    {
        int count = 0;

        public EmptyScript(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Update(FrameTime deltaTime)
        {
            count++;

            if(count > 1000)
            {
                count = 0;
            }
        }
    }
}
