using System.Numerics;

namespace Fletch.Engine.Model.Config
{
    public class PhysicsWorldConfig
    {
        public Vector2 Gravity { get; }

        public PhysicsWorldConfig(Vector2 gravity)
        {
            Gravity = gravity;
        }

        public PhysicsWorldConfig()
        {
            Gravity = new Vector2(0, -9.81f);
        }
    }
}
