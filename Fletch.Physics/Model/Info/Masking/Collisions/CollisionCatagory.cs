namespace Fletch.Physics.Model.Info.Masking.Collisions
{
    [Flags]
    public enum CollisionCategory : ulong
    {
        None = 0,

        // World geometry
        Static = 1UL << 0,
        Dynamic = 1UL << 1,
        OneWayPlatform = 1UL << 2,
        MovingPlatform = 1UL << 3,
        WorldBoundary = 1UL << 4,
        BreakableWorld = 1UL << 5,

        // Characters
        Player = 1UL << 6,
        FriendlyCharacter = 1UL << 7,
        EnemyCharacter = 1UL << 8,
        NeutralCharacter = 1UL << 9,

        // Character-related shapes
        CharacterHitbox = 1UL << 10,
        CharacterHurtbox = 1UL << 11,
        CharacterSensor = 1UL << 12,

        // Physical objects
        Prop = 1UL << 13,
        PushableObject = 1UL << 14,
        CarryableObject = 1UL << 15,
        Debris = 1UL << 16,
        Ragdoll = 1UL << 17,

        // Items
        Pickup = 1UL << 18,
        DroppedItem = 1UL << 19,

        // Combat
        PlayerProjectile = 1UL << 20,
        EnemyProjectile = 1UL << 21,
        NeutralProjectile = 1UL << 22,
        MeleeAttack = 1UL << 23,
        AreaAttack = 1UL << 24,
        Shield = 1UL << 25,

        // Vehicles
        Vehicle = 1UL << 26,
        VehicleWheel = 1UL << 27,
        VehicleSensor = 1UL << 28,

        // General detection volumes
        Trigger = 1UL << 29,
        InteractionSensor = 1UL << 30,
        GroundSensor = 1UL << 31,
        VisionSensor = 1UL << 32,
        ProximitySensor = 1UL << 33,

        // Environmental volumes
        FluidVolume = 1UL << 34,
        ForceVolume = 1UL << 35,
        HazardVolume = 1UL << 36,

        // Specialized physics
        Rope = 1UL << 37,
        Chain = 1UL << 38,
        SoftBody = 1UL << 39,
        Particle = 1UL << 40,

        // Query filtering
        RaycastTarget = 1UL << 41,
        Occluder = 1UL << 42,
        NavigationObstacle = 1UL << 43,
        CameraObstacle = 1UL << 44,

        // Special collision behavior
        Ghost = 1UL << 45,
        PassThrough = 1UL << 46,
        ReservedPhysics = 1UL << 47,

        // Project-specific expansion
        Custom01 = 1UL << 48,
        Custom02 = 1UL << 49,
        Custom03 = 1UL << 50,
        Custom04 = 1UL << 51,
        Custom05 = 1UL << 52,
        Custom06 = 1UL << 53,
        Custom07 = 1UL << 54,
        Custom08 = 1UL << 55,
        Custom09 = 1UL << 56,
        Custom10 = 1UL << 57,
        Custom11 = 1UL << 58,
        Custom12 = 1UL << 59,
        Custom13 = 1UL << 60,
        Custom14 = 1UL << 61,
        Custom15 = 1UL << 62,
        Custom16 = 1UL << 63,

        All = ulong.MaxValue
    }
}
