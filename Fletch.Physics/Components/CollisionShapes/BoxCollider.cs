using Fletch.Physics.Abstractions.CollisionShapes;
using Fletch.Physics.Model;
using Fletch.Physics.Model.ResourceHandles;
using System.Numerics;

namespace Fletch.Physics.Components.CollisionShapes
{
    public class BoxCollider : Collider
    {
        public float Width { get => width; set => SetFieldAndDirty(ref width, value); }

        public float Height { get => height; set => SetFieldAndDirty(ref height, value); }

        private float width;

        private float height;

        internal BoxCollider(
            float width,
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
            this.width = width;
            this.height = height;
        }
    }
}