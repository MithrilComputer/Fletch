namespace Fletch.Physics.Model
{
    [Flags]
    internal enum RigidBodyDirtyFlags
    {
        None = 0,

        Pose = 1 << 0,

        GravityScale = 1 << 1,

        LinearDrag = 1 << 2,

        AngularDrag = 1 << 3,

        LinearVelocity = 1 << 4,

        AngularVelocity = 1 << 5,

        MotionLocks = 1 << 6,

        PhysicsMode = 1 << 7,

        Interpolation = 1 << 8,

        EnabledState = 1 << 9,

        All = Pose | GravityScale | LinearDrag | AngularDrag |
              LinearVelocity | AngularVelocity | MotionLocks | PhysicsMode | Interpolation
    }
}
