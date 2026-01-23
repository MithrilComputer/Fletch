using Fletch.Core.Components.Update;
using Fletch.Core.Diagnostics;
using Fletch.Engine.Components;
using Fletch.Engine.Components.Updateable;
using Fletch.Engine.Hierarchy;
using Fletch.Engine.Model;
using Fletch.Engine.Scenes;

namespace Fletch.Engine.Systems
{
    internal class ScriptRunnerSystem : SceneSubsystem, IUpdateable, IFixedUpdateable
    {
        // TODO, optimize script storage and lookup, prob use a HashSet, perhpase somthing to avoid checking non enabled scripts

        private readonly List<Script> scripts = new List<Script>();

        private readonly Queue<Script> startQueue = new Queue<Script>();
        private readonly Queue<Script> lateStartQueue = new Queue<Script>();

        private readonly Queue<Script> pendingAdds = new Queue<Script>();
        private readonly Queue<Script> pendingRemoves = new Queue<Script>();

        private readonly IFletchContextLogger<ScriptRunnerSystem> logger;

        public ScriptRunnerSystem(IFletchContextLogger<ScriptRunnerSystem> logger)
        {
            this.logger = logger;
        }

        public override void AttachToScene(Scene scene)
        {
            base.AttachToScene(scene);

            scene.SystemScheduler.RegisterSystem(this, SystemExecutionOrder.Update, 0);

            scene.AddSystemComponentRegistration(typeof(Script), (a, b, c) => OnScriptChange(a,b,c)); //TODO, I need to make a try add so I dont double register later
        }

        public void FixedUpdate(float deltaTime)
        {
            FlushPendingAddsAndRemoves();

            for (int i = 0; i < scripts.Count; i++)
            {
                Script script = scripts[i];
                if (script.IsEnabled)
                    script.FixedUpdate(deltaTime);
            }
        }

        public void Update(float deltaTime)
        {

            FlushPendingAddsAndRemoves();

            RunStartQueue();

            for (int i = 0; i < scripts.Count; i++)
            {
                Script script = scripts[i];
                if (script.IsEnabled)
                    script.Update(deltaTime);
            }

            RunLateStartQueue();
        }

        private void FlushPendingAddsAndRemoves()
        {
            while (pendingAdds.Count > 0)
            {
                Script addingScript = pendingAdds.Dequeue();
                scripts.Add(addingScript);
            }
            while (pendingRemoves.Count > 0)
            {
                Script removingScript = pendingRemoves.Dequeue();
                scripts.Remove(removingScript);
            }
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

                if(!startingScript.HasLateStarted)
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
            if (script == null)
            {
                logger.LogWarning("Cant add a null script. Did you register a null?");
                return this;
            }

            if (scripts.Contains(script) || pendingAdds.Contains(script))
            {
                logger.LogWarning("Cant add an already existing script. Did you register it twice?");
                return this;
            }

            startQueue.Enqueue(script);
            pendingAdds.Enqueue(script);

            return this;
        }

        public ScriptRunnerSystem RemoveScriptComponent(Script script)
        {
            if (script == null)
            {
                logger.LogWarning("Cant remove a null script.");
                return this;
            }

            if (!scripts.Contains(script) || pendingRemoves.Contains(script))
            {
                logger.LogWarning("Cant remove a script that is not on the list. Did you forget to register it?");
                return this;
            }

            pendingRemoves.Enqueue(script);

            return this;
        }

        private void OnScriptChange(GameObject gameObject, Component script, ComponentChangeType changeType)
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
