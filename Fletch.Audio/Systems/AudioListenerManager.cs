using Fletch.Audio.Abstractions.Backend;
using Fletch.Audio.Components;
using Fletch.Audio.Model.SoundListenerCommands;
using Fletch.Engine.Model;

namespace Fletch.Audio.Systems
{
    internal class AudioListenerManager : IDisposable
    {
        private readonly TrackedSet<AudioListener> audioListeners = new TrackedSet<AudioListener>();

        private readonly IAudioBackend audioBackend;

        private AudioListener? activeListener = null;

        private bool disposed;

        public AudioListenerManager(IAudioBackend audioBackend)
        {
            this.audioBackend = audioBackend;
        }

        public void AddListener(AudioListener listener)
        {
            audioListeners.MarkToAdd(listener);
        }

        public void RemoveListener(AudioListener listener)
        {
            audioListeners.MarkToRemove(listener);
        }

        public void Update()
        {
            if (disposed)
            {
                return;
            }

            audioListeners.Refresh();

            audioListeners.Sort(ListenerCompare);

            activeListener = null;

            foreach (AudioListener audioListener in audioListeners.Items)
            {
                if (audioListener.IsEnabled)
                {
                    activeListener = audioListener;
                    break;
                }
            }

            if (activeListener != null && activeListener.IsEnabled)
            {
                SetListenerGainCommand gainCommand = new SetListenerGainCommand(activeListener.Gain); //TODO Use Dirty detection to avoid sending this every frame.
                SetListenerPositionCommand positionCommand = new SetListenerPositionCommand(activeListener.GameObject.Transform.WorldPosition); //TODO Use Dirty detection to avoid sending this every frame.

                audioBackend.SendCommand(gainCommand); //TODO Use Dirty detection to avoid sending this every frame.
                audioBackend.SendCommand(positionCommand); //TODO Use Dirty detection to avoid sending this every frame.
            }
            else
            {
                SetListenerGainCommand gainCommand = new SetListenerGainCommand(0f); //TODO Use Dirty detection to avoid sending this every frame.

                audioBackend.SendCommand(gainCommand); //TODO Use Dirty detection to avoid sending this every frame.
            }
        }

        public void RefreshListeners()
        {
            if (disposed)
            {
                return;
            }

            audioListeners.Refresh();

            audioListeners.Sort(ListenerCompare);
        }

        /// <summary>
        /// Compares listeners by priority.
        /// </summary>
        /// <param name="a">First listener.</param>
        /// <param name="b">Second listener.</param>
        /// <returns>Sort comparison.</returns>
        private static int ListenerCompare(AudioListener a, AudioListener b)
        {
            return b.Priority.CompareTo(a.Priority);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            audioListeners.Clear();
        }
    }
}
