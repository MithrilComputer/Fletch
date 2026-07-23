using Box2D.NET;
using Fletch.Physics.Model;
using Fletch.Physics.Model.Info.Masking.Collisions;
using System.Numerics;

namespace Fletch.Physics.Box2D.Helpers
{
    internal static class FletchB2Converter
    {
        public static B2BodyType ConvertToB2(PhysicsMode physicsMode)
        {
            switch (physicsMode)
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

        public static Vector2 ConvertToFletch(B2Vec2 b2vector)
        {
            return new Vector2(b2vector.X, b2vector.Y);
        }

        public static float B2RotToRad(B2Rot rotation)
        {
            return MathF.Atan2(rotation.s, rotation.c);
        }

        public static B2Filter B2Filter(CollisionFilter filter)
        {
            return new B2Filter((ulong)filter.Category, (ulong)filter.Mask, filter.GroupIndex);
        }
    }
}
