using Fletch.Physics.Abstractions.CollisionShapes;
using Fletch.Physics.Model;
using Fletch.Physics.Model.Info.Collisions;
using Fletch.Physics.Model.ResourceHandles;
using System.Numerics;

namespace Fletch.Physics.Components.CollisionShapes
{
    public class CircleCollider : Collider
    {
        public float Radius { get => radius; set => SetFieldAndDirty(ref radius, value); }

        private float radius;

        internal CircleCollider(
            float radius,
            IBodyHandle parentBody,
            IColliderHandle colliderHandle,
            PhysicsMaterial material = new(),
            Vector2 offset = default)
            : base(
                  colliderHandle,
                  parentBody,
                  material,
                  offset)
        {
            this.radius = radius;
        }
    }
}
