using Box2D.NET;
using Fletch.Physics.Model;

namespace Fletch.Physics.Box2D.Factories
{
    internal static class ShapeDefinitionFactory
    {
        public static B2ShapeDef CreateShapeDef(PhysicsMaterial material, bool isSensor, B2Filter filter)
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

            shapeDef.filter = filter;

            shapeDef.material.friction = material.Friction;

            shapeDef.material.restitution = material.Restitution;

            return shapeDef;
        }
    }
}
