namespace Fletch.Core.Time
{
    /// <summary>
    /// Represents timing information for a fixed-step update.
    /// Used for deterministic simulation such as physics.
    /// </summary>
    public readonly struct FixedTimeStep
    {
        /// <summary>
        /// The fixed delta time for this step.
        /// This value is constant for every fixed update.
        /// </summary>
        public float Delta { get; }

        /// <summary>
        /// The total simulated time since the runtime started.
        /// Advances only during fixed updates.
        /// </summary>
        public TimeSpan Total { get; }

        /// <summary>
        /// Creates a new fixed time step instance.
        /// </summary>
        public FixedTimeStep(float delta, TimeSpan total)
        {
            Delta = delta;
            Total = total;
        }
    }
}
