using Box2D.NET;
using Fletch.Core.EngineConfig;
using Fletch.Physics.Box2D.Model.ResourceHandles;
using Fletch.Physics.Model;
using Fletch.Physics.Model.Info.Shapes;
using Fletch.Physics.Model.ResourceHandles;
using System.Numerics;

namespace Fletch.Physics.Box2D.Natives
{
    internal class Box2DManager
    {
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

        public IBodyHandle CreateBody(IWorldHandle worldHandle, Vector2 Position, float rotation, PhysicsMode physicsMode)
        {
            if (worldHandle is not Box2DWorldHandle b2dWorldHandle)
                throw new Exception();

            B2BodyDef bodyDef = new B2BodyDef();

            bodyDef.position = new B2Vec2(Position.X, Position.Y);

            bodyDef.rotation = new B2Rot(MathF.Cos(rotation), MathF.Sin(rotation));

            bodyDef.type = ConvertToB2BodyType(physicsMode);

            B2BodyId bodyId = B2Bodies.b2CreateBody(b2dWorldHandle.Id, in bodyDef);

            return new Box2DBodyHandle(bodyId);
        }

        public IColliderHandle CreateShape(IBodyHandle body, PhysicsMaterial physicsMaterial, ShapeData shapeData, Vector2 offset, float rotation)
        {
            switch (shapeData)
            {
                case RectangleData rectData:

                    break;

                case CircleData circleData:

                    break;

                case CapsuleData capsuleData:

                    Vector2 centerOne = new Vector2(MathF.Cos(rotation), MathF.Sin(rotation));
                    Vector2 centerTwo = new Vector2(-MathF.Cos(rotation), -MathF.Sin(rotation));

                    centerOne *= capsuleData.Height / 2;
                    centerTwo *= capsuleData.Height / 2;

                    centerOne += offset;
                    centerTwo += offset;

                    B2Vec2 b2CenterOne = new B2Vec2(centerOne.X, centerOne.Y);
                    B2Vec2 b2CenterTwo = new B2Vec2(centerOne.X, centerOne.Y);

                    B2Capsule capsule = new B2Capsule(b2CenterOne, b2CenterTwo, capsuleData.Radius);

                    break;
            }

        }

        private B2BodyType ConvertToB2BodyType(PhysicsMode physicsMode)
        {
            switch(physicsMode)
            {
                case PhysicsMode.Static:
                    return B2BodyType.b2_staticBody;

                case PhysicsMode.Kenematic:
                    return B2BodyType.b2_kinematicBody;

                case PhysicsMode.Dynamic:
                    return B2BodyType.b2_dynamicBody;

                default: throw new Exception();
            }
        }
    }
}
