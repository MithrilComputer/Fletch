using Fletch.Audio.Components;
using Fletch.Engine.Model;

namespace Fletch.Audio.Systems
{
    internal class AudioListenerManager
    {
        private readonly TrackedSet<AudioListener> audioListeners = new TrackedSet<AudioListener>();

        private AudioListener? activeListener = null;

    }
}
