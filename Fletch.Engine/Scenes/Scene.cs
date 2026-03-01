using Fletch.Core.Diagnostics;
using Fletch.Core.Time;
using Fletch.Engine.Abstractions.Factories;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy;
using Fletch.Engine.Model;
using Fletch.Engine.Systems;

namespace Fletch.Engine.Scenes
{
    public sealed class Scene : IDisposable
    {
        internal SubSystemManager SystemManager { get; }

        private uint nextId;
    
        private readonly Queue<uint> freedIds = new Queue<uint>();

        private readonly Dictionary<Type, List<Action< GameObjectComponent, ComponentChangeType>>> componentCallBacks
            = new Dictionary<Type, List<Action<GameObjectComponent, ComponentChangeType>>>();

        private readonly List<GameObject> gameObjects = new List<GameObject>();

        private readonly IGameObjectFactory gameObjectFactory;

        private readonly IFletchContextLogger<Scene> logger;

        internal Scene(IFletchContextLogger<Scene> logger, IGameObjectFactory gameObjectFactory, ISubSystemFactory subSystemFactory)
        {
            this.logger = logger;
            
            this.gameObjectFactory = gameObjectFactory;

            SystemManager = new SubSystemManager(subSystemFactory, this);
        }

        public void Update(FrameTime frameTime)
        {
            SystemManager.UpdateSystems(frameTime.Delta);
        }

        public void FixedUpdate(FixedTimeStep fixedTime)
        {
            SystemManager.UpdateFixedSystems(fixedTime);
        }

        public void Render(FrameTime frameTime)
        {
            SystemManager.UpdateRenderables(frameTime);
        }

        /// <summary>
        /// Used to register a component change event to a <see cref="SceneSubsystem"/>. Only use this method inside the <see cref="SceneSubsystem.AttachToScene(Scene)"/> of a <see cref="SceneSubsystem"/> to avoid misuse.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddSystemComponentRegistration(
            Type componentType,
            Action<GameObjectComponent, ComponentChangeType> callback)
        {
            if (componentType == null) 
                throw new ArgumentNullException(nameof(componentType));

            if (callback == null) 
                throw new ArgumentNullException(nameof(callback));

            if (!componentCallBacks.TryGetValue(componentType, out List<Action<GameObjectComponent, ComponentChangeType>> list))
            {
                list = new List<Action<GameObjectComponent, ComponentChangeType>>();

                componentCallBacks.Add(componentType, list);
            }

            list.Add(callback);
        }

        public void RemoveSystemComponentRegistration(
            Type componentType,
            Action<GameObjectComponent, ComponentChangeType> callback)
        {
            if (componentType == null) throw new ArgumentNullException(nameof(componentType));

            if (callback == null) throw new ArgumentNullException(nameof(callback));

            if (!componentCallBacks.TryGetValue(componentType, out List<Action<GameObjectComponent, ComponentChangeType>> list))
            {
                logger.LogWarning("Cant Remove A System Component Registration That has not been registered. Did you forget to register the system component?");

                return;
            }

            list.Remove(callback);

            if (list.Count == 0)
                componentCallBacks.Remove(componentType);
        }

        public void OnComponentChange(GameObjectComponent component, ComponentChangeType changeType)
        {
            if (component.GameObject == null || component == null)
                return;

            Type? componentType = component.GetType();

            if (componentCallBacks.TryGetValue(componentType, out var callbacks))
            {
                foreach (var callback in callbacks.ToArray())
                    callback(component, changeType);
            }
        }

        public GameObject CreateGameObject()
        {
            uint id;

            if (freedIds.Count > 0)
            {
                id = freedIds.Dequeue();
            }
            else
            {
                id = nextId++;
            }

            GameObject gameObject = gameObjectFactory.BuildGameObject(id);

            gameObjects.Add(gameObject);

            gameObject.ComponentChanged += OnComponentChange;

            return gameObject;
        }

        public void DestroyGameObject(GameObject gameObject)
        {
            if(gameObject == null)
            {
                logger.LogWarning("Cant Destroy A Null GameObject.");
                return;
            }

            if(!gameObjects.Contains(gameObject))
            {
                logger.LogWarning("Cant Destroy A Non-Registered GameObject.");
                return;
            }

            gameObjects.Remove(gameObject);

            freedIds.Enqueue(gameObject.ID);

            gameObject.ComponentChanged -= OnComponentChange;

            gameObject.Dispose();
        }

        public void Dispose()
        {
            for (int i = gameObjects.Count - 1; i >= 0; i--)
            {
                DestroyGameObject(gameObjects[i]);
            }

            componentCallBacks.Clear();
            freedIds.Clear();

            SystemManager.Dispose();
        }
    }
}
