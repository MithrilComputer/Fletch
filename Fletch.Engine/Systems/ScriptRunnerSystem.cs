using Fletch.Core.Diagnostics;
using Fletch.Core.LifeCycle;
using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Model;
using Fletch.Engine.Systems.Scenes;

namespace Fletch.Engine.Systems
{
    internal class ScriptRunnerSystem : SceneSubsystem, IUpdateable, IFixedUpdateable
    {
        private readonly TrackedSet<Script> scripts = new TrackedSet<Script>();

        private readonly Queue<Script> startQueue = new Queue<Script>();
        private readonly Queue<Script> lateStartQueue = new Queue<Script>();

        private readonly IFletchContextLogger<ScriptRunnerSystem> logger;

        public ScriptRunnerSystem(IFletchContextLogger<ScriptRunnerSystem> logger)
        {
            this.logger = logger;
        }

        public override void AttachToScene(Scene scene)
        {
            base.AttachToScene(scene);

            scene.SystemManager.RegisterSystem(this, SystemExecutionOrder.Update, 0);

            scene.AddSystemComponentRegistration(typeof(Script), (b, c) => OnScriptChange(b, c)); //TODO, I need to make a try add so I dont double register later
        }

        public void FixedUpdate(FixedTimeStep deltaTime)
        {
            scripts.Refresh();

            for (int i = 0; i < scripts.Items.Count; i++)
            {
                Script script = scripts.Items[i];
                if (script.IsEnabled && script.HasStarted)
                    script.FixedUpdate(deltaTime);
            }
        }

        public void Update(FrameTime deltaTime)
        {

            scripts.Refresh();

            RunStartQueue();

            for (int i = 0; i < scripts.Items.Count; i++)
            {
                Script script = scripts.Items[i];

                if (script.IsEnabled && script.HasStarted)
                    script.Update(deltaTime);
            }

            RunLateStartQueue();
        }

        private void RunStartQueue()
        {
            while (startQueue.Count > 0)
            {
                Script startingScript = startQueue.Dequeue();

                if (!startingScript.IsEnabled || startingScript.HasStarted)
                    continue;

                startingScript.HasStarted = true;
                startingScript.OnStart();

                if (!startingScript.HasLateStarted)
                    lateStartQueue.Enqueue(startingScript);
            }
        }

        private void RunLateStartQueue()
        {
            while (lateStartQueue.Count > 0)
            {
                Script startingScript = lateStartQueue.Dequeue();

                if (!startingScript.IsEnabled || startingScript.HasLateStarted)
                    continue;

                startingScript.HasLateStarted = true;
                startingScript.OnLateStart();
            }
        }

        public ScriptRunnerSystem AddScriptComponent(Script script)
        {
            scripts.MarkToAdd(script);
            startQueue.Enqueue(script);

            return this;
        }

        public ScriptRunnerSystem RemoveScriptComponent(Script script)
        {
            scripts.MarkToRemove(script);
            return this;
        }

        private void OnScriptChange(GameObjectComponent script, ComponentChangeType changeType)
        {
            switch (changeType)
            {
                case ComponentChangeType.Added:
                    AddScriptComponent((Script)script);
                    break;
                case ComponentChangeType.Removed:
                    RemoveScriptComponent((Script)script);
                    break;
                case ComponentChangeType.Enabled:
                    // Do nothing for now
                    break;
                case ComponentChangeType.Disabled:
                    // Do nothing for now
                    break;
            }
        }
    }
}
