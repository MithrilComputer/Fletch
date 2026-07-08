using Box2D.NET;
using Fletch.Core.EngineConfig;
using Fletch.Physics.Box2D.Factories;
using Fletch.Physics.Box2D.Helpers;
using Fletch.Physics.Box2D.Model.ResourceHandles;
using Fletch.Physics.Model.Info.IO;
using Fletch.Physics.Model.ResourceHandles;
using System.Numerics;

namespace Fletch.Physics.Box2D.Natives
{
    internal class Box2DManager
    {
        private readonly Box2DObjectStateManager objectStateManager;

        public Box2DManager()
        {
            objectStateManager = new Box2DObjectStateManager();
        }

        public void Step(IWorldHandle worldHandle, float delta)
        {
            if (worldHandle is not Box2DWorldHandle b2Handle)
                throw new Exception();

            B2Worlds.b2World_Step(b2Handle.Id, delta, EngineConfig.PhysicsSubStepCount);
        }

        public IWorldHandle CreateWorld(Vector2 gravity)
        {
            B2WorldDef worldDef = new B2WorldDef();

            worldDef.gravity = new B2Vec2(gravity.X, gravity.Y);

            B2WorldId worldId = B2Worlds.b2CreateWorld(in worldDef);

            return new Box2DWorldHandle(worldId);
        }

        public IBodyHandle CreateBody(IWorldHandle worldHandle, BodyWriteState writeState)
        {
            if (worldHandle is not Box2DWorldHandle b2dWorldHandle)
                throw new Exception();

            B2BodyDef newBodyDef = new B2BodyDef();

            B2BodyId newBodyId = B2Bodies.b2CreateBody(b2dWorldHandle.Id, in newBodyDef);

            objectStateManager.WriteStateToBody(newBodyId, writeState);

            return new Box2DBodyHandle(newBodyId);
        }

        public IColliderHandle CreateCollider(IBodyHandle body, ColliderWriteState writeState)
        {
            if (body is not Box2DBodyHandle b2BodyHandle)
                throw new Exception();

            B2ShapeDef newShapeDef = ShapeDefinitionFactory.CreateShapeDef(writeState.Material, writeState.IsSensor, new B2Filter()); //TODO Implement Filter 

            B2ShapeId newShapeId = B2Shapes.b2CreatePolygonShape(b2BodyHandle.Id, newShapeDef, new B2Polygon());

            objectStateManager.SetShapeData(newShapeId, writeState.Shape, writeState.Rotation, writeState.Offset);

            return new Box2DColiderHandle(newShapeId);
        }

        public void DestroyResource(IWorldHandle world)
        {
            if (world is not Box2DWorldHandle b2WorldHandle)
                throw new Exception();

            B2Worlds.b2DestroyWorld(b2WorldHandle.Id);
        }

        public void DestroyResource(IBodyHandle body)
        {
            if (body is not Box2DBodyHandle b2BodyHandle)
                throw new Exception();

            B2Bodies.b2DestroyBody(b2BodyHandle.Id);
        }

        public void DestroyResource(IColliderHandle colliderHandle)
        {
            if (colliderHandle is not Box2DColiderHandle b2ColliderHandle)
                throw new Exception();

            B2Shapes.b2DestroyShape(b2ColliderHandle.Id, true);
        }


        public void SetBodyState(IBodyHandle bodyHandle, in BodyWriteState writeState)
        {
            if (bodyHandle is not Box2DBodyHandle b2BodyHandle)
                throw new Exception();

            objectStateManager.WriteStateToBody(b2BodyHandle.Id, writeState);
        }

        public BodyReadState GetBodyState(IBodyHandle bodyHandle)
        {
            if (bodyHandle is not Box2DBodyHandle b2BodyHandle)
                throw new Exception();

            Vector2 position = 
                FletchB2Converter.ConvertToFletch(
                    B2Bodies.b2Body_GetPosition(b2BodyHandle.Id));

            float roation = FletchB2Converter.B2RotToRad(
                B2Bodies.b2Body_GetRotation(b2BodyHandle.Id));

            Vector2 linearVelocity = FletchB2Converter.ConvertToFletch(
                B2Bodies.b2Body_GetLinearVelocity(b2BodyHandle.Id));

            float angularVelocity = B2Bodies.b2Body_GetAngularVelocity(b2BodyHandle.Id);

            bool isAwake = B2Bodies.b2Body_IsAwake(b2BodyHandle.Id);

            return new BodyReadState(
                position,
                roation,
                linearVelocity,
                angularVelocity,
                isAwake
                );
        }

        public void SetColiderState(IColliderHandle colliderHandle, ColliderWriteState writeState)
        {
            if (colliderHandle is not Box2DColiderHandle b2colliderHandle)
                throw new Exception();

            objectStateManager.WriteStateToCollider(b2colliderHandle, writeState);
        }
    }
}
