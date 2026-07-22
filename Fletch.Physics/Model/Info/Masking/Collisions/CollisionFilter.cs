namespace Fletch.Physics.Model.Info.Masking.Collisions
{
    /// <summary>
    /// Filters collisions based on category and mask. This class is used to determine which objects can collide with each other in the physics simulation.
    /// </summary>
    internal class CollisionFilter
    {
        /// <summary>
        /// The category of the object. This is used to determine which other objects it can collide with.
        /// </summary>
        public CollisionCategory Category { get; }

        /// <summary>
        /// The mask of the object. This is used to determine which categories of objects it can collide with.
        /// </summary>
        public CollisionCategory Mask { get; }

        /// <summary>
        /// Same positive group always collides.
        /// Same negative group never collides.
        /// Zero means normal category/mask filtering.
        /// </summary>
        public int GroupIndex { get; }

        public CollisionFilter(CollisionCategory category, CollisionCategory mask, int groupIndex = 0)
        {
            Category = category;
            Mask = mask;
            GroupIndex = groupIndex;
        }
    }
}
