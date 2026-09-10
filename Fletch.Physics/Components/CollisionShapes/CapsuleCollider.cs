using Fletch.Physics.Abstractions.CollisionShapes;
using Fletch.Physics.Model;
using Fletch.Physics.Model.ResourceHandles;
using System.Numerics;

namespace Fletch.Physics.Components.CollisionShapes
{
    public class CapsuleCollider : Collider
    {
        public float Radius { get => radius; set => SetFieldAndDirty(ref radius, value); }

        public float Height { get => height; set => SetFieldAndDirty(ref height, value); }

        private float radius;

        private float height;

        internal CapsuleCollider(
            float radius, 
            float height, 
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
            this.height = height;
        }
    }
}
