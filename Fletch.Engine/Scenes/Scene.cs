using Fletch.Core.Components.Update;
using Fletch.Core.LifeCycle;
using Fletch.Engine.Components.Updateable;
using Fletch.Engine.Hierarchy;
using Fletch.Engine.Model;
using Fletch.Engine.Systems;

namespace Fletch.Engine.Scenes
{
    public class Scene
    {
        private readonly List<IUpdateable> updateables = new List<IUpdateable>();
        private readonly List<IFixedUpdateable> fixedUpdateables = new List<IFixedUpdateable>();

        private readonly Queue<IStartable> pendingStart = new Queue<IStartable>();
        private readonly Queue<ILateStartable> lateStartables = new Queue<ILateStartable>();

        private readonly List<SystemEntry> subSystems = new List<SystemEntry>();

        public readonly List<GameObject> gameObjects = new List<GameObject>();

        // TODO: Add Scene subsystem ordering.
        // Use a SystemPhase enum (Input, FixedUpdate, Update, LateUpdate, Render)
        // plus an int OrderOffset for fine-grain before/after control.
        // Scene should rebuild per-phase execution lists on add/remove
        // and execute phases in a fixed order.

        //TODO Add an ability for subSystems to register their component types to the Scene, so when a comp is added or removed, the right system gets the right info and can react
    }
}
