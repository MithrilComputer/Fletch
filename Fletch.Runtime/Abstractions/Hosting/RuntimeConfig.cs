namespace Fletch.Runtime.Abstractions.Hosting
{
    /// <summary>
    /// Configuration for runtime host behavior (timing and pause policy).
    /// </summary>
    internal sealed class RuntimeConfig
    {
        //TODO Make this class usefull later
        /// <summary>
        /// If true, fixed updates (physics-style) are executed using an accumulator.
        /// Variable updates still run once per frame.
        /// </summary>
        public bool EnableFixedUpdates { get; init; } = true;

        /// <summary>
        /// The fixed delta time used for each fixed update step.
        /// Common values: 1/60s, 1/50s, 1/120s.
        /// </summary>
        public TimeSpan FixedTimeStep { get; init; } = TimeSpan.FromSeconds(1.0 / 60.0);

        /// <summary>
        /// Maximum number of fixed steps that may be processed in a single frame.
        /// Prevents the "spiral of death" when the game lags.
        /// </summary>
        public int MaxFixedStepsPerFrame { get; init; } = 5;

        /// <summary>
        /// If true, incoming frame delta time is clamped to <see cref="MaxFrameDeltaTime"/>.
        /// Useful to avoid huge delta times after stalls (alt-tab, window drag, breakpoint).
        /// </summary>
        public bool ClampFrameDeltaTime { get; init; } = true;

        /// <summary>
        /// Maximum allowed delta time for a single frame when clamping is enabled.
        /// Typical range: 0.1s to 0.25s.
        /// </summary>
        public TimeSpan MaxFrameDeltaTime { get; init; } = TimeSpan.FromSeconds(0.25);

        /// <summary>
        /// If true, the host pauses simulation updates while the window is not focused.
        /// Fixed updates and variable updates are skipped while paused.
        /// Rendering can still occur depending on <see cref="RenderWhilePaused"/>.
        /// </summary>
        public bool PauseOnFocusLost { get; init; } = true;

        /// <summary>
        /// If true, the host still calls Render while paused.
        /// Useful for pause menus and UI.
        /// </summary>
        public bool RenderWhilePaused { get; init; } = true;

        /// <summary>
        /// Optional global time scale applied by the host to variable updates.
        /// Fixed updates are typically NOT scaled to keep physics stable.
        /// </summary>
        public float VariableTimeScale { get; init; } = 1.0f;
    }
}
