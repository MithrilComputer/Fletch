using Box2D.NET;
using Fletch.Physics.Model.ResourceHandles;

namespace Fletch.Physics.Box2D.Model.ResourceHandles
{
    internal class Box2DBodyHandle : IBodyHandle
    {
        public B2BodyId Id { get; }

        public Box2DBodyHandle(B2BodyId bodyID)
        {
            this.Id = bodyID;
        }
    }
}
