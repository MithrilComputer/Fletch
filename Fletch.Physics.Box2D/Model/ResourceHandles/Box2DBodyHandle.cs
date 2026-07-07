using Box2D.NET;
using Fletch.Physics.Model.ResourceHandles;

namespace Fletch.Physics.Box2D.Model.ResourceHandles
{
    internal class Box2DBodyHandle : IBodyHandle
    {
        B2BodyId bodyID { get; }

        public Box2DBodyHandle(B2BodyId bodyID)
        {
            this.bodyID = bodyID;
        }
    }
}
