using Box2D.NET;
using System.Numerics;

namespace Fletch.Physics.Box2D.Factories
{
    internal static class ShapeFactory
    {
        public static B2Polygon CreateRectangle(Vector2 size, Vector2 offset, float rotation)
        {
            return B2Geometries.b2MakeOffsetBox(
                size.X / 2, 
                size.Y / 2, 
                new B2Vec2(offset.X, offset.Y),
                new B2Rot(MathF.Cos(rotation), MathF.Sin(rotation)));
        }

        public static B2Circle CreateCircle(float radius, Vector2 offset)
        {
            return new B2Circle(new B2Vec2(offset.X, offset.Y), radius);
        }

        public static B2Capsule CreateCapsule(float segmentLength, float radius, float rotation, Vector2 offset)
        {
            Vector2 centerOne = new Vector2(MathF.Cos(rotation), MathF.Sin(rotation));
            Vector2 centerTwo = new Vector2(-MathF.Cos(rotation), -MathF.Sin(rotation));

            centerOne *= segmentLength / 2;
            centerTwo *= segmentLength / 2;

            centerOne += offset;
            centerTwo += offset;

            B2Vec2 b2CenterOne = new B2Vec2(centerOne.X, centerOne.Y);
            B2Vec2 b2CenterTwo = new B2Vec2(centerTwo.X, centerTwo.Y);

            return new B2Capsule(b2CenterOne, b2CenterTwo, radius);
        }

    }
}
