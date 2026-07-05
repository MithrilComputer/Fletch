using Fletch.Core.Components.Update;
using Fletch.Core.LifeCycle;
using Fletch.Core.Time;
using Fletch.Engine.Abstractions.Factories;
using Fletch.Engine.Components.Updateable;
using Fletch.Engine.Model;
using Fletch.Engine.Scenes;

namespace Fletch.Engine.Systems
{
    internal sealed class SubSystemManager : IDisposable
    {
        private readonly List<SystemEntry> systemEntries = new List<SystemEntry>();

        private readonly Scene loadedScene;

        private readonly ISubSystemFactory subSystemFactory;

        private int executionOrderIndex = 0;

        public SubSystemManager(ISubSystemFactory subSystemFactory, Scene loadedScene)
        {
            this.subSystemFactory = subSystemFactory;
            this.loadedScene = loadedScene;
        }

        public void AddSubSystem<T>() where T : SceneSubsystem
        {
            T instance = subSystemFactory.CreateSubSystem<T>();

            instance.AttachToScene(loadedScene);
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

        public void UpdateFixedSystems(FixedTimeStep deltaTime)
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

        public void UpdateRenderables(FrameTime frameTime)
        {
            for (int i = 0; i < systemEntries.Count; i++)
            {
                SceneSubsystem system = systemEntries[i].System;

                if (system is IRenderable renderables)
                {
                    renderables.Render(frameTime);
                }
            }
        }

        private void DetachSubSystemsFromScene()
        {
            for (int i = 0; i < systemEntries.Count; i++)
            {
                systemEntries[i].System.DetachFromScene();
            }
        }

        public void RegisterSystem(SceneSubsystem system, SystemExecutionOrder phase, int order)
        {
            if (system == null)
                throw new ArgumentNullException(nameof(system));

            SystemEntry entry = new SystemEntry(system, new ExecutionOrderInfo(phase, order, executionOrderIndex));

            systemEntries.Add(entry);

            executionOrderIndex++;

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
            DetachSubSystemsFromScene();

            foreach (var entry in systemEntries)
            {
                if (entry.System is IDisposable disposable)
                {
                    disposable.Dispose();// for now just dispose, later Detach From Scene might do more
                }
            }

            systemEntries.Clear();
        }
    }
}
