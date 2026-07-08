using Box2D.NET;
using Fletch.Physics.Model.ResourceHandles;

namespace Fletch.Physics.Box2D.Model.ResourceHandles
{
    internal class Box2DColiderHandle : IColliderHandle
    {
        public B2ShapeId Id { get; private set; }

        public Box2DColiderHandle(B2ShapeId Id)
        {
            this.Id = Id;
        }

        public void OverideId(B2ShapeId Id)
        {
            this.Id = Id;
        }
    }
}
