namespace Fletch.Audio.Model.Flags
{
    [Flags]
    internal enum AudioListenerDirtyFlags
    {
        None = 0,

        Gain = 1 << 0,

        Position = 1 << 1,

        All = Gain
    }
}
