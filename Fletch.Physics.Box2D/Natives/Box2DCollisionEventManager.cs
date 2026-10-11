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

        public IReadOnlyCollection<BackendCollisionEvent> GetCollisionEvents(B2WorldId worldId)
        {
            B2ContactEvents contactEvents =
                B2Worlds.b2World_GetContactEvents(worldId);

            int beginCount = contactEvents.beginCount;
            int endCount = contactEvents.endCount;

            BackendCollisionEvent[] eventInfos =
                new BackendCollisionEvent[beginCount + endCount];

            for (int i = 0; i < beginCount; i++)
            {
                B2ContactBeginTouchEvent contact = contactEvents.beginEvents[i];

                if (!colliderRegistry.TryGetColliderHandleFromShapeId(
                        contact.shapeIdA,
                        out Box2DColiderHandle aColliderHandle))
                {
                    throw new InvalidOperationException(
                        "Could not find collider for contact shape A.");
                }

                if (!colliderRegistry.TryGetColliderHandleFromShapeId(
                        contact.shapeIdB,
                        out Box2DColiderHandle bColliderHandle))
                {
                    throw new InvalidOperationException(
                        "Could not find collider for contact shape B.");
                }

                eventInfos[i] = new BackendCollisionEvent(
                    aColliderHandle,
                    bColliderHandle,
                    CollisionEventType.Enter);
            }

            for (int i = 0; i < endCount; i++)
            {
                B2ContactEndTouchEvent contact = contactEvents.endEvents[i];

                if (!colliderRegistry.TryGetColliderHandleFromShapeId(
                        contact.shapeIdA,
                        out Box2DColiderHandle aColliderHandle))
                {
                    throw new InvalidOperationException(
                        "Could not find collider for contact shape A.");
                }

                if (!colliderRegistry.TryGetColliderHandleFromShapeId(
                        contact.shapeIdB,
                        out Box2DColiderHandle bColliderHandle))
                {
                    throw new InvalidOperationException(
                        "Could not find collider for contact shape B.");
                }

                eventInfos[beginCount + i] = new BackendCollisionEvent(
                    aColliderHandle,
                    bColliderHandle,
                    CollisionEventType.Exit);
            }

            return eventInfos;
        }
    }
}
