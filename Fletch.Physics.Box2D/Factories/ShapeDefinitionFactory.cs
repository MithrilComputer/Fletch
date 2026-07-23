using Box2D.NET;
using Fletch.Physics.Box2D.Helpers;
using Fletch.Physics.Model;
using Fletch.Physics.Model.Info.Masking.Collisions;

namespace Fletch.Physics.Box2D.Factories
{
    internal static class ShapeDefinitionFactory
    {
        public static B2ShapeDef CreateShapeDef(PhysicsMaterial material, bool isSensor, CollisionFilter filter)
        {
            B2ShapeDef shapeDef = new B2ShapeDef();

            if (isSensor)
            {
                shapeDef.isSensor = true;
                shapeDef.enableSensorEvents = true;
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
