using Fletch.Audio.Abstractions.Backend;
using Fletch.Audio.Components;
using Fletch.Audio.Factories.SoundPlayers;
using Fletch.Audio.Model;
using Fletch.Audio.Model.SoundPlayerCommands.Source;
using Fletch.Engine.Model;

namespace Fletch.Audio.Systems
{
    internal class AudioPlaybackComponentManager : IDisposable
    {
        private readonly TrackedSet<AudioSource> audioSources = new TrackedSet<AudioSource>();

        private readonly TrackedSet<SoundPlayer> soundPlayers = new TrackedSet<SoundPlayer>();

        private readonly IAudioBackend audioBackend;

        private readonly SoundPlayerFactory soundPlayerFactory;

        private bool disposed;

        public void AddComponentToPool(AudioSource component)
        {
            audioSources.MarkToAdd(component);
        }

        public void AddComponentToPool(SoundPlayer component)
        {
            soundPlayers.MarkToAdd(component);
        }

        public void Update()
        {
            if (disposed)
            {
                return;
            }

            audioListeners.Refresh();

            audioListeners.Sort(ListenerCompare);

            audioSources.Refresh();

            foreach (SoundPlayer removedSoundPlayer in soundPlayers.PendingRemoves)
            {
                if (removedSoundPlayer.SourceHandle == null)
                    continue;

                ReleaseSourceCommand sourceRelease = new ReleaseSourceCommand(removedSoundPlayer.SourceHandle);

                audioBackend.SendCommand(sourceRelease);
            }

            soundPlayers.Refresh();

            UpdatePlayers();

            UpdateListeners();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }
            disposed = true;
            audioSources.Clear();
            soundPlayers.Clear();
        }
    }
}
