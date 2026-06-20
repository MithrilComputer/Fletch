using System.Numerics;

namespace Fletch.Audio.Model
{
    internal interface ISoundListener
    {
        Vector2 CurrentPosition { get; }

        float CurrentGain { get; }
    }
}
