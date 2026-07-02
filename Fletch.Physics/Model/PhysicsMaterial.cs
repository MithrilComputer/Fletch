namespace Fletch.Physics.Model
{
    public readonly struct PhysicsMaterial
    {
        public float Friction { get; }

        public float Restitution { get; }

        public PhysicsMaterial(float friction = 0.2f, float restitution = 0f)
        {
            Friction = friction;
            Restitution = restitution;
        }
    }
}
