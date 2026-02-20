namespace Fletch.Core.Time
{
    public readonly struct FrameTime
    {
        public float Delta { get; }
        public TimeSpan Total { get; }

        public float? Alpha { get; }

        public FrameTime(float delta, TimeSpan total)
        {
            Delta = delta;
            Total = total;
        }

        public FrameTime(float delta, TimeSpan total, float alpha)
        {
            Delta = delta;
            Total = total;
            Alpha = alpha;
        }
    }
}
