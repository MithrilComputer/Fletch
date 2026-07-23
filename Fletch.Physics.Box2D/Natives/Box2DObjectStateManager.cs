using Box2D.NET;
using Fletch.Physics.Box2D.Factories;
using Fletch.Physics.Box2D.Helpers;
using Fletch.Physics.Box2D.Model.ResourceHandles;
using Fletch.Physics.Box2D.Registries;
using Fletch.Physics.Model.Info.IO;
using Fletch.Physics.Model.Info.Shapes;
using System.Numerics;

namespace Fletch.Physics.Box2D.Natives
{
    internal class Box2DObjectStateManager
    {
        private readonly ColliderRegistry colliderRegistry;

        public Box2DObjectStateManager(ColliderRegistry colliderRegistry)
        {
            this.colliderRegistry = colliderRegistry;
        }

        public void WriteStateToCollider(Box2DColiderHandle colliderHandle, ColliderWriteState writeState)
        {
            if ((writeState.DirtyFlags & ColliderDirtyFlags.IsSensor) != 0)
            {
                RebuildCollider(
                    colliderHandle,
                    writeState
                    );

                return;
            }

            if ((writeState.DirtyFlags & ColliderDirtyFlags.Material) != 0)
            {
                B2Shapes.b2Shape_SetFriction(
                    colliderHandle.Id,
                    writeState.Material.Friction
                    );

                B2Shapes.b2Shape_SetRestitution(
                    colliderHandle.Id,
                    writeState.Material.Restitution
                    );
            }

            if((writeState.DirtyFlags & ColliderDirtyFlags.Shape) != 0)
            {
                SetShapeData(
                    colliderHandle.Id,
                    writeState.Shape,
                    writeState.Rotation,
                    writeState.Offset
                    );
            }

            if ((writeState.DirtyFlags & ColliderDirtyFlags.Filter) != 0)
            {
                B2Shapes.b2Shape_SetFilter(
                    colliderHandle.Id,
                    FletchB2Converter.B2Filter(writeState.Filter)
                    );
            }
        }

        public void WriteStateToBody(B2BodyId b2BodyId, BodyWriteState writeState)
        {
            B2Vec2 position = new B2Vec2(writeState.Position.X, writeState.Position.Y);

            B2Rot rotation = new B2Rot(MathF.Cos(writeState.Rotation), MathF.Sin(writeState.Rotation));

            B2Bodies.b2Body_SetType(b2BodyId, FletchB2Converter.ConvertToB2(writeState.Mode));

            B2Bodies.b2Body_SetTransform(b2BodyId, position, rotation);

            B2Bodies.b2Body_SetLinearVelocity(b2BodyId,
                new B2Vec2(
                    writeState.LinearVelocity.X,
                    writeState.LinearVelocity.Y
                    ));

            B2Bodies.b2Body_SetAngularVelocity(b2BodyId, writeState.AngularVelocity);

            B2Bodies.b2Body_SetGravityScale(b2BodyId, writeState.GravityScale);

            B2Bodies.b2Body_SetLinearDamping(b2BodyId, writeState.LinearDamping);

            B2Bodies.b2Body_SetAngularDamping(b2BodyId, writeState.AngularDamping);

            B2Bodies.b2Body_SetMotionLocks(b2BodyId,
                new B2MotionLocks(
                    writeState.LockX,
                    writeState.LockY,
                    writeState.FixedRotation
                    ));

            if (writeState.Enabled)
            {
                B2Bodies.b2Body_Enable(b2BodyId);
            }
            else
            {
                B2Bodies.b2Body_Disable(b2BodyId);
            }
        }

        private void RebuildCollider(Box2DColiderHandle coliderHandle, ColliderWriteState writeState)
        {
            B2ShapeId oldShapeId = coliderHandle.Id;

            B2BodyId bodyId = B2Shapes.b2Shape_GetBody(oldShapeId);

            B2ShapeDef shapeDef = ShapeDefinitionFactory.CreateShapeDef(writeState.Material, writeState.IsSensor, writeState.Filter);

            B2ShapeId newShapeId = B2Shapes.b2CreatePolygonShape(bodyId, shapeDef, new B2Polygon()); //new B2Polygon() is a shape that will be replaced, it is a place holder.

            colliderRegistry.RemoveRegistration(oldShapeId);

            B2Shapes.b2DestroyShape(oldShapeId, true);

            coliderHandle.OverideId(newShapeId);

            SetShapeData(newShapeId, writeState.Shape, writeState.Rotation, writeState.Offset);

            colliderRegistry.RegisterCollider(coliderHandle);
        }

        public void SetShapeData(B2ShapeId shapeId, ShapeData shapeData, float rotation, Vector2 offset)
        {
            switch (shapeData)
            {
                case RectangleData rectangleData:

                    B2Polygon rectangleShape = ShapeFactory.CreateRectangle(rectangleData.Size, offset, rotation);

                    B2Shapes.b2Shape_SetPolygon(shapeId, ref rectangleShape);

                    break;

                case CircleData circleData:

                    B2Circle circleShape = ShapeFactory.CreateCircle(circleData.Radius, offset);

                    B2Shapes.b2Shape_SetCircle(shapeId, in circleShape);

                    break;

                case CapsuleData capsuleData:

                    B2Capsule capsuleShape = ShapeFactory.CreateCapsule(capsuleData.Height, capsuleData.Radius, rotation, offset);

                    B2Shapes.b2Shape_SetCapsule(shapeId, in capsuleShape);

                    break;

                default: throw new NotImplementedException();
            }
        }
    }
}
