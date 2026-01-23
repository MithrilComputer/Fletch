using Fletch.Core.Components.Update;
using Fletch.Engine.Components.Updateable;
using Fletch.Engine.Model;
using Fletch.Engine.Scenes;

namespace Fletch.Engine.Systems
{
    public sealed class SystemScheduler : IDisposable
    {
        private readonly List<SystemEntry> systemEntries = new List<SystemEntry>();

        private Scene? loadedScene;

        private int executionOrderIndex = 0;

        public SystemScheduler()
        {

        }

        public void UpdateSystems(float deltaTime)
        {
            for (int i = 0; i < systemEntries.Count; i++)
            {
                SceneSubsystem system = systemEntries[i].System;

                if (system is IUpdateable updateable)
                {
                    updateable.Update(deltaTime);
                }
            }
        }

        public void UpdateFixedSystems(float deltaTime)
        {
            for (int i = 0; i < systemEntries.Count; i++)
            {
                SceneSubsystem system = systemEntries[i].System;

                if (system is IFixedUpdateable fixedUpdateable)
                {
                    fixedUpdateable.FixedUpdate(deltaTime);
                }
            }
        }

        public void SetLoadedScene(Scene scene)
        {
            if (scene == null)
                throw new ArgumentNullException(nameof(scene));

            if (loadedScene == scene)
                return;

            if (loadedScene != null)
            {
                for (int i = 0; i < systemEntries.Count; i++)
                {
                    systemEntries[i].System.DetachFromScene();
                }
            }

            loadedScene = scene;

            for (int i = 0; i < systemEntries.Count; i++)
            {
                systemEntries[i].System.AttachToScene(scene);
            }
        }

        public void RegisterSystem(SceneSubsystem system, SystemExecutionOrder phase, int order)
        {
            if (system == null)
                throw new ArgumentNullException(nameof(system));

            var entry = new SystemEntry(system, new ExecutionOrderInfo(phase, order, executionOrderIndex));
            systemEntries.Add(entry);

            executionOrderIndex++;

            if (loadedScene != null)
                system.AttachToScene(loadedScene);

            systemEntries.Sort(SystemCompare);
        }

        public T GetSystem<T>() where T : SceneSubsystem
        {
            for (int i = 0; i < systemEntries.Count; i++)
            {
                if (systemEntries[i].System is T match)
                    return match;
            }

            throw new KeyNotFoundException($"System not registered: {typeof(T).Name}");
        }

        public void OrderSystems()
        {
            systemEntries.Sort(SystemCompare);
        }

        private static int SystemCompare(SystemEntry a, SystemEntry b)
        {
            int phase = a.ExecutionOrder.Phase.CompareTo(b.ExecutionOrder.Phase);
            if (phase != 0)
                return phase;

            int order = a.ExecutionOrder.Order.CompareTo(b.ExecutionOrder.Order);
            if (order != 0)
                return order;

            return a.ExecutionOrder.RegistrationIndex.CompareTo(b.ExecutionOrder.RegistrationIndex);
        }

        public void Dispose()
        {
            foreach (var entry in systemEntries)
            {
                if(entry.System is IDisposable disposable)
                {
                    disposable.Dispose();// for now just dispose, later Detach From Scene might do more
                }
            }

            systemEntries.Clear();
        }
    }
}
