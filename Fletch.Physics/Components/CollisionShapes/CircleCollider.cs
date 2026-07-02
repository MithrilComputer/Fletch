using Fletch.Engine.Components;
using Fletch.Physics.Model;

namespace Fletch.Physics.Components.CollisionShapes
{
    public class CircleCollider : GameObjectComponent, ICollisionShape
    {
        public float Radius;

        public PhysicsMaterial Material { get; } = new PhysicsMaterial();

    }
}
