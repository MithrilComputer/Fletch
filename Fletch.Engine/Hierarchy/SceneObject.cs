using Fletch.Engine.Systems.Scenes;

namespace Fletch.Engine.Hierarchy
{
    public abstract class SceneObject
    {
        public string Name { get; set; }

        public uint ID { get; internal set; }

        protected Scene HostScene { get; }

        public bool IsDisposed { get; private set; }

        protected SceneObject(uint id, string name, Scene hostScene)
        {
            ID = id;
            Name = name;
            HostScene = hostScene;
        }

        public abstract void Destroy();

        internal void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            OnDispose();
        }

        protected virtual void OnDispose() { }
    }
}
