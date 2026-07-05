using Fletch.Physics.Model;
using Fletch.Physics.Model.Info.Collisions;

namespace Fletch.Physics.Components.CollisionShapes
{
    public class CapsuleCollider : CollisionShape
    {
        public float Width;

        public float Height;

        public override PhysicsMaterial Material { get; set; } = new PhysicsMaterial();

        public override event Action<CollisionEventInfo>? CollisionEnter;
        public override event Action<CollisionEventInfo>? CollisionExit;
    }
}
