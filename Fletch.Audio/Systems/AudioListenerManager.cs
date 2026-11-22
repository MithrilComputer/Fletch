using Fletch.Audio.Abstractions.Backend;
using Fletch.Audio.Components;
using Fletch.Audio.Model.Flags;
using Fletch.Audio.Model.SoundListenerCommands;
using Fletch.Engine.Model;

namespace Fletch.Audio.Systems
{
    internal class AudioListenerManager : IDisposable
    {
        private readonly TrackedSet<AudioListener> audioListeners = new TrackedSet<AudioListener>();

        private readonly IAudioBackend audioBackend;

        private AudioListener? activeListener = null;

        private bool lastListenerEnabledState = false;

        private bool disposed;

        public AudioListenerManager(IAudioBackend audioBackend)
        {
            this.audioBackend = audioBackend;

            SetListenerGainCommand gainCommand = new SetListenerGainCommand(0f);

            audioBackend.SendCommand(gainCommand);
        }

        public void AddListener(AudioListener listener)
        {
            audioListeners.MarkToAdd(listener);
        }

        public void RemoveListener(AudioListener listener)
        {
            audioListeners.MarkToRemove(listener);
        }

        /// <summary>
        /// Refreshes the list of audio listeners, sorts them by priority, and updates the active listener in the audio backend.
        /// </summary>
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
                lastListenerEnabledState = true;

                if (activeListener.DirtyFlags == 0)
                    return;

                if ((activeListener.DirtyFlags & AudioListenerDirtyFlags.Gain) != 0)
                {
                    SetListenerGainCommand gainCommand = new SetListenerGainCommand(activeListener.Gain);
                    audioBackend.SendCommand(gainCommand);
                }

                if ((activeListener.DirtyFlags & AudioListenerDirtyFlags.Position) != 0)
                {
                    SetListenerPositionCommand positionCommand = new SetListenerPositionCommand(activeListener.GameObject.Transform.WorldPosition);
                    audioBackend.SendCommand(positionCommand);
                }

                activeListener.ClearDirty();
            }
            else
            {
                if (!lastListenerEnabledState)
                    return;

                SetListenerGainCommand gainCommand = new SetListenerGainCommand(0f);
                audioBackend.SendCommand(gainCommand);

                lastListenerEnabledState = false;
            }
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
