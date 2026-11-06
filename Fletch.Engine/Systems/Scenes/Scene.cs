using Fletch.Core.Diagnostics;
using Fletch.Core.Time;
using Fletch.Engine.Abstractions.Factories;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy;
using Fletch.Engine.Model;

namespace Fletch.Engine.Systems.Scenes
{
    public sealed class Scene : IDisposable
    {
        internal SubSystemManager SystemManager { get; }

        private uint nextId;

        private readonly Queue<uint> freedIds = new Queue<uint>();

        private readonly Dictionary<Type, List<Action<Component, ComponentChangeType>>> componentCallBacks
            = new Dictionary<Type, List<Action<Component, ComponentChangeType>>>();

        private bool disposed = false;

        private readonly List<GameObject> gameObjects = new List<GameObject>();

        private readonly HashSet<GameObject> pendingDestruction = new HashSet<GameObject>();

        private readonly IGameObjectFactory gameObjectFactory;

        private readonly IFletchContextLogger<Scene> logger;

        internal Scene(IFletchContextLogger<Scene> logger, IGameObjectFactory gameObjectFactory, ISubSystemFactory subSystemFactory)
        {
            this.logger = logger;

            this.gameObjectFactory = gameObjectFactory;

            SystemManager = new SubSystemManager(subSystemFactory, this);
        }

        /// <summary>
        /// Occurs first on each update, before everything else.
        /// </summary>
        public event Action<FrameTime>? OnUpdate;

        /// <summary>
        /// Occurs first on each fixed update, before everything else.
        /// </summary>
        public event Action<FixedTimeStep>? OnFixedUpdate;

        internal void Update(FrameTime frameTime)
        {
            if (disposed)
                return;

            OnUpdate?.Invoke(frameTime);

            SystemManager.UpdateSystems(frameTime);

            DestroyObjects();
        }

        internal void FixedUpdate(FixedTimeStep fixedTime)
        {
            if (disposed)
                return;

            SystemManager.UpdateFixedSystems(fixedTime);

            OnFixedUpdate?.Invoke(fixedTime);
        }

        internal void Render(FrameTime frameTime)
        {
            if (disposed)
                return;

            SystemManager.UpdateRenderables(frameTime);
        }

        /// <summary>
        /// Used to register a component change event to a <see cref="SceneSubsystem"/>. Only use this method inside the <see cref="SceneSubsystem.AttachToScene(Scene)"/> of a <see cref="SceneSubsystem"/> to avoid misuse.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        internal void AddSystemComponentRegistration(
            Type componentType,
            Action<Component, ComponentChangeType> callback)
        {
            if (disposed)
                return;

            if (componentType == null)
                throw new ArgumentNullException(nameof(componentType));

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            if (!componentCallBacks.TryGetValue(componentType, out List<Action<Component, ComponentChangeType>> list))
            {
                list = new List<Action<Component, ComponentChangeType>>();

                componentCallBacks.Add(componentType, list);
            }

            list.Add(callback);
        }

        internal void RemoveSystemComponentRegistration(
            Type componentType,
            Action<Component, ComponentChangeType> callback)
        {
            if (disposed)
                return;

            if (componentType == null) throw new ArgumentNullException(nameof(componentType));

            if (callback == null) throw new ArgumentNullException(nameof(callback));

            if (!componentCallBacks.TryGetValue(componentType, out List<Action<Component, ComponentChangeType>> list))
            {
                logger.LogWarning("Cant Remove A System Component Registration That has not been registered. Did you forget to register the system component?");

                return;
            }

            list.Remove(callback);

            if (list.Count == 0)
                componentCallBacks.Remove(componentType);
        }

        internal void OnComponentChange(Component component, ComponentChangeType changeType)
        {
            if (disposed)
                return;

            if (component == null)
                return;

            Type concreteType = component.GetType();

            foreach (KeyValuePair<Type, List<Action<Component, ComponentChangeType>>> registration in componentCallBacks)
            {
                if (!registration.Key.IsAssignableFrom(concreteType))
                    continue;

                foreach (Action<Component, ComponentChangeType> callback in registration.Value.ToArray())
                {
                    callback(component, changeType);
                }
            }
        }

        public GameObject CreateGameObject()
        {
            if (disposed)
                return null;

            uint id;

            if (freedIds.Count > 0)
            {
                id = freedIds.Dequeue();
            }
            else
            {
                id = nextId++;
            }

            GameObject gameObject = gameObjectFactory.BuildGameObject(id, this);

            gameObjects.Add(gameObject);

            gameObject.ComponentChanged += OnComponentChange;

            return gameObject;
        }

        internal void DestroyObjects()
        {
            if (disposed)
                return;

            while (pendingDestruction.Count > 0)
            {
                SceneObject[] objectsToDestroy = pendingDestruction.ToArray();

                pendingDestruction.Clear();

                foreach (SceneObject gameObject in objectsToDestroy)
                {
                    if (!gameObjects.Contains(gameObject))
                        continue;

                    gameObject.Dispose();

                    gameObject.ComponentChanged -= OnComponentChange;

                    gameObjects.Remove(gameObject);

                    freedIds.Enqueue(gameObject.ID);
                }
            }
        }

        internal void MarkObjectForDistruction(GameObject gameObject)
        {
            if (disposed)
                return;

            if (!gameObjects.Contains(gameObject))
                return;

            pendingDestruction.Add(gameObject);
        }

        public void Dispose()
        {
            if (disposed)
                return;

            for (int i = gameObjects.Count - 1; i >= 0; i--)
            {
                MarkGameObjectForDistruction(gameObjects[i]);
            }

            DestroyObjects();

            componentCallBacks.Clear();
            freedIds.Clear();

            SystemManager.Dispose();

            disposed = true;
        }
    }
}
