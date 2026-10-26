
using Box2D.NET;
using Fletch.Physics.Box2D.Model.ResourceHandles;
using Fletch.Physics.Box2D.Registries;
using Fletch.Physics.Model.Info.Collisions;
using System.Net;

namespace Fletch.Physics.Box2D.Natives
{
    internal class Box2DCollisionEventManager
    {
        private readonly ColliderRegistry colliderRegistry;

        private readonly List<BackendCollisionEvent> eventInfos =
                new List<BackendCollisionEvent>();

        public Box2DCollisionEventManager(ColliderRegistry colliderRegistry)
        {
            this.colliderRegistry = colliderRegistry;
        }

        public IReadOnlyCollection<BackendCollisionEvent> GetCollisionEvents(B2WorldId worldId)
        {
            eventInfos.Clear();

            CollectContactEvents(worldId);
            CollectSensorEvents(worldId);

            return eventInfos;
        }
        
        private void CollectSensorEvents(B2WorldId worldId)
        {
            B2SensorEvents sensorEvents =
                B2Worlds.b2World_GetSensorEvents(worldId);

            int beginCount = sensorEvents.beginCount;
            int endCount = sensorEvents.endCount;

            for (int i = 0; i < beginCount; i++)
            {
                B2SensorBeginTouchEvent contact = sensorEvents.beginEvents[i];

                if (!colliderRegistry.TryGetColliderHandleFromShapeId(
                        contact.sensorShapeId,
                        out Box2DColiderHandle? aColliderHandle))
                {
                    throw new InvalidOperationException(
                        "Could not find collider for contact shape A.");
                }

                if (!colliderRegistry.TryGetColliderHandleFromShapeId(
                        contact.visitorShapeId,
                        out Box2DColiderHandle? bColliderHandle))
                {
                    throw new InvalidOperationException(
                        "Could not find collider for contact shape B.");
                }

                eventInfos.Add(new BackendCollisionEvent(
                    aColliderHandle,
                    bColliderHandle,
                    CollisionEventType.Enter,
                    true));
            }

            for (int i = 0; i < endCount; i++)
            {
                B2SensorEndTouchEvent contact = sensorEvents.endEvents[i];

                if (!colliderRegistry.TryGetColliderHandleFromShapeId(
                        contact.sensorShapeId,
                        out Box2DColiderHandle? aColliderHandle))
                {
                    continue;
                }

                if (!colliderRegistry.TryGetColliderHandleFromShapeId(
                        contact.visitorShapeId,
                        out Box2DColiderHandle? bColliderHandle))
                {
                    continue;
                }

                eventInfos.Add(new BackendCollisionEvent(
                    aColliderHandle,
                    bColliderHandle,
                    CollisionEventType.Exit,
                    true));
            }
        }

        private void CollectContactEvents(B2WorldId worldId)
        {
            B2ContactEvents contactEvents =
                B2Worlds.b2World_GetContactEvents(worldId);

            int beginCount = contactEvents.beginCount;
            int endCount = contactEvents.endCount;

            for (int i = 0; i < beginCount; i++)
            {
                B2ContactBeginTouchEvent contact = contactEvents.beginEvents[i];

                if (!colliderRegistry.TryGetColliderHandleFromShapeId(
                        contact.shapeIdA,
                        out Box2DColiderHandle? aColliderHandle))
                {
                    throw new InvalidOperationException(
                        "Could not find collider for contact shape A.");
                }

                if (!colliderRegistry.TryGetColliderHandleFromShapeId(
                        contact.shapeIdB,
                        out Box2DColiderHandle? bColliderHandle))
                {
                    throw new InvalidOperationException(
                        "Could not find collider for contact shape B.");
                }

                eventInfos.Add(new BackendCollisionEvent(
                    aColliderHandle,
                    bColliderHandle,
                    CollisionEventType.Enter,
                    false));
            }

            for (int i = 0; i < endCount; i++)
            {
                B2ContactEndTouchEvent contact = contactEvents.endEvents[i];

                if (!colliderRegistry.TryGetColliderHandleFromShapeId(
                        contact.shapeIdA,
                        out Box2DColiderHandle? aColliderHandle))
                {
                    continue;
                }

                if (!colliderRegistry.TryGetColliderHandleFromShapeId(
                        contact.shapeIdB,
                        out Box2DColiderHandle? bColliderHandle))
                {
                    continue;
                }

                eventInfos.Add(new BackendCollisionEvent(
                    aColliderHandle,
                    bColliderHandle,
                    CollisionEventType.Exit,
                    false));
            }
        }
    }
}
