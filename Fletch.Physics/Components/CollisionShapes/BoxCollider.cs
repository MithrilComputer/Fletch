using Fletch.Engine.Components;
using Fletch.Physics.Model;

namespace Fletch.Physics.Components.CollisionShapes
{
    public class BoxCollider: GameObjectComponent, ICollisionShape
    {
        public float Width;

        public float Height;

        public PhysicsMaterial Material { get; } = new PhysicsMaterial();
    }
}
