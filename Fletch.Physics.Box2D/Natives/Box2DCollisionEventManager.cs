using Box2D.NET;
using Fletch.Physics.Model.Info.Collisions;
using Fletch.Physics.Model.ResourceHandles;

namespace Fletch.Physics.Box2D.Natives
{
    internal class Box2DCollisionEventManager
    {
        public IReadOnlyCollection<B2ContactEvents> GetCollisionEvents(B2WorldId worldId)
        {
            B2ContactEvents contactEvents = B2Worlds.b2World_GetContactEvents(worldId);

            int beginCount = contactEvents.beginCount;

            int endCount = contactEvents.endCount;

            int totalCount = beginCount + endCount;

            CollisionEventInfo[] evenInfos = new CollisionEventInfo[totalCount];

            for (int i = 0; contactEvents.beginCount > 0; i++)
            {
                contactEvents.beginEvents[i]

                new CollisionEventInfo();
            }
        }

    }
}
