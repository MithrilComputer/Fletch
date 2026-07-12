using Box2D.NET;
using Fletch.Physics.Box2D.Model.ResourceHandles;
using Fletch.Physics.Box2D.Registries;
using Fletch.Physics.Model.Info.Collisions;

namespace Fletch.Physics.Box2D.Natives
{
    internal class Box2DCollisionEventManager
    {
        private readonly ColliderRegistry colliderRegistry;

        public Box2DCollisionEventManager(ColliderRegistry colliderRegistry)
        {
            this.colliderRegistry = colliderRegistry;
        }

        public IReadOnlyCollection<CollisionEventInfo> GetCollisionEvents(B2WorldId worldId)
        {
            B2ContactEvents contactEvents = B2Worlds.b2World_GetContactEvents(worldId);

            int beginCount = contactEvents.beginCount;

            int endCount = contactEvents.endCount;

            int totalCount = beginCount + endCount;

            CollisionEventInfo[] evenInfos = new CollisionEventInfo[totalCount];

            for (int i = 0; i < beginCount; i++)
            {
                B2ShapeId contactA = contactEvents.beginEvents[i].shapeIdA;
                B2ShapeId contactB = contactEvents.beginEvents[i].shapeIdB;

                if(!colliderRegistry.TryGetColliderHandleFromShapeId(contactA, out Box2DColiderHandle aColliderHandle))
                {
                    throw new Exception();
                }

                if(!colliderRegistry.TryGetColliderHandleFromShapeId(contactB, out Box2DColiderHandle bColliderHandle))
                {
                    throw new Exception();
                }

                evenInfos[i] = new CollisionEventInfo(aColliderHandle, bColliderHandle, CollisionEventType.Enter);
            }

            for (int i = beginCount - 1; i < endCount; i++)
            {
                B2ShapeId contactA = contactEvents.endEvents[i].shapeIdA;
                B2ShapeId contactB = contactEvents.endEvents[i].shapeIdB;

                if (!colliderRegistry.TryGetColliderHandleFromShapeId(contactA, out Box2DColiderHandle aColliderHandle))
                {
                    throw new Exception();
                }

                if (!colliderRegistry.TryGetColliderHandleFromShapeId(contactB, out Box2DColiderHandle bColliderHandle))
                {
                    throw new Exception();
                }

                evenInfos[i] = new CollisionEventInfo(aColliderHandle, bColliderHandle, CollisionEventType.Exit);
            }

            return evenInfos;
        }
    }
}
