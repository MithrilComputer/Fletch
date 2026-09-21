using Box2D.NET;
using Fletch.Physics.Box2D.Helpers;
using Fletch.Physics.Model;
using Fletch.Physics.Model.Info.Masking.Collisions;
using System.Diagnostics;

namespace Fletch.Physics.Box2D.Factories
{
    internal static class ShapeDefinitionFactory
    {
        public static B2ShapeDef CreateShapeDef(PhysicsMaterial material, bool isSensor, CollisionFilter filter, float mass)
        {
            B2ShapeDef shapeDef = B2Types.b2DefaultShapeDef();

            Debug.Print($"Mass {mass}");

            shapeDef.density = mass;

            if (isSensor)
            {
                shapeDef.isSensor = true;
                shapeDef.enableSensorEvents = true;
                shapeDef.density = 0f;
            }
            else
            {
                shapeDef.isSensor = false;
                shapeDef.enableSensorEvents = false;
            }

            shapeDef.filter = FletchB2Converter.B2Filter(filter);

            shapeDef.material.friction = material.Friction;

            shapeDef.material.restitution = material.Restitution;

            return shapeDef;
        }
    }
}
