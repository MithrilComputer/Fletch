using System;
using System.Collections.Generic;
using System.Text;

namespace Fletch.Audio.Model.Flags
{
    [Flags]
    internal enum SoundPlayerDirtyFlags
    {
        None = 0,

        Position = 1 << 0,

        Volume = 1 << 1,

        Spatialization = 1 << 2,

        Looping = 1 << 3,

        Pitch = 1 << 4,

        All = Position | Volume | Spatialization | Looping | Pitch
    }
}
