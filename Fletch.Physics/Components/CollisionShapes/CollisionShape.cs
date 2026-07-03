using Fletch.Engine.Components;
using Fletch.Physics.Model;
using Fletch.Physics.Model.Info.Collisions;

namespace Fletch.Physics.Components.CollisionShapes
{
    public abstract class CollisionShape : GameObjectComponent
    {
        public abstract PhysicsMaterial Material { get; set; }

        public abstract event Action<CollisionEventInfo>? CollisionEnter;
        public abstract event Action<CollisionEventInfo>? CollisionExit;
    }
}
