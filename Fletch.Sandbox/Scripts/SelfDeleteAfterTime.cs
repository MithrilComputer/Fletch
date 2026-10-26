using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy;

namespace Fletch.Sandbox.Scripts
{
    internal class SelfDeleteAfterTime : Script
    {
        private float timeKeep = 0f;

        private float timeToDestruction = 0f;
        
        private bool started = false;


        public SelfDeleteAfterTime(GameObject gameObject) : base(gameObject) {}

        public override void Update(FrameTime deltaTime)
        {
            if(started)
            {
                timeKeep += deltaTime.Delta;

                if(timeKeep >= timeToDestruction)
                {
                    GameObject.Destroy();
                }
            }
        }

        public void Start(float timeToDestruction)
        {
            if (started)
                return;

            started = true;

            this.timeToDestruction = timeToDestruction;
        }
    }
}
