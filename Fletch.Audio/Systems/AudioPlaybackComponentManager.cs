using Fletch.Audio.Abstractions.Backend;
using Fletch.Audio.Components;
using Fletch.Audio.Factories.SoundPlayers;
using Fletch.Audio.Model;
using Fletch.Audio.Model.Flags;
using Fletch.Audio.Model.SoundPlayerCommands;
using Fletch.Audio.Model.SoundPlayerCommands.Source;
using Fletch.Engine.Model;
using System.Numerics;

namespace Fletch.Audio.Systems
{
    internal class AudioPlaybackComponentManager : IDisposable
    {
        private readonly TrackedSet<AudioSource> audioSources = new TrackedSet<AudioSource>();

        private readonly TrackedSet<SoundPlayer> soundPlayers = new TrackedSet<SoundPlayer>();

        private readonly IAudioBackend audioBackend;

        private readonly SoundPlayerFactory soundPlayerFactory;

        private bool disposed;

        public AudioPlaybackComponentManager(IAudioBackend audioBackend)
        {
            this.audioBackend = audioBackend;
            soundPlayerFactory = new SoundPlayerFactory(audioBackend);
        }

        /// <summary>
        /// Adds an audio source component to the manager.
        /// </summary>
        public void AddComponent(AudioSource component)
        {
            if (disposed)
            {
                return;
            }

            audioSources.MarkToAdd(component);
        }

        /// <summary>
        /// Adds an audio source component to the manager.
        /// </summary>
        public void AddComponent(SoundPlayer component)
        {
            if (disposed)
            {
                return;
            }

            soundPlayers.MarkToAdd(component);
        }

        /// <summary>
        /// Removes an audio source component from the manager.
        /// </summary>
        public void RemoveComponent(AudioSource component)
        {
            if (disposed)
            {
                return;
            }

            audioSources.MarkToRemove(component);
        }

        /// <summary>
        /// Removes an audio source component from the manager.
        /// </summary>
        public void RemoveComponent(SoundPlayer component)
        {
            if (disposed)
            {
                return;
            }

            soundPlayers.MarkToRemove(component);
        }

        /// <summary>
        /// The Update method is for updating the state of audio sources and sound players.
        /// </summary>
        public void Update()
        {
            if (disposed)
            {
                return;
            }

            foreach (SoundPlayer soundPlayer in soundPlayers.Items)
            {
                if (soundPlayer.ParentAudioSource == null)
                {
                    continue;
                }

                if (soundPlayer.SourceHandle == null)
                {
                    continue;
                }

                if(soundPlayer.DirtyFlags == 0)
                {
                    continue;
                }

                if((soundPlayer.DirtyFlags & SoundPlayerDirtyFlags.Position) != 0)
                {
                    Vector2 position = soundPlayer.ParentAudioSource.GameObject.Transform.WorldPosition;
                    SetPlayerPositionCommand setPositionCommand = new SetPlayerPositionCommand(soundPlayer.SourceHandle, position);

                    audioBackend.SendCommand(setPositionCommand);
                }

                if ((soundPlayer.DirtyFlags & SoundPlayerDirtyFlags.Pitch) != 0)
                {
                    SetPlayerPitchCommand pitchCommand = new SetPlayerPitchCommand(soundPlayer.SourceHandle, soundPlayer.Pitch);

                    audioBackend.SendCommand(pitchCommand);
                }

                if ((soundPlayer.DirtyFlags & SoundPlayerDirtyFlags.Looping) != 0)
                {
                    SetPlayerLoopingCommand loopingCommand = new SetPlayerLoopingCommand(soundPlayer.SourceHandle, soundPlayer.IsLooping);

                    audioBackend.SendCommand(loopingCommand);
                }

                if ((soundPlayer.DirtyFlags & SoundPlayerDirtyFlags.Volume) != 0)
                {
                    SetPlayerVolumeCommand volumeCommand = new SetPlayerVolumeCommand(soundPlayer.SourceHandle, soundPlayer.Volume);

                    audioBackend.SendCommand(volumeCommand);
                }

                if ((soundPlayer.DirtyFlags & SoundPlayerDirtyFlags.Spatialization) != 0)
                {
                    SetIsPlayerSpatialCommand spatialCommand = new SetIsPlayerSpatialCommand(soundPlayer.SourceHandle, soundPlayer.IsSpatial);

                    audioBackend.SendCommand(spatialCommand);
                }

                soundPlayer.ClearDirty();
            }
        }

        /// <summary>
        /// Requests a new sound player.
        /// </summary>
        /// <param name="soundKey">Sound asset key.</param>
        /// <returns>New sound player.</returns>
        public SoundPlayer RequestNewSoundPlayer(string soundKey)
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            SoundPlayer soundPlayer = soundPlayerFactory.Create(soundKey);

            soundPlayers.MarkToAdd(soundPlayer);

            return soundPlayer;
        }

        /// <summary>
        /// Releases a sound player.
        /// </summary>
        /// <param name="soundPlayer">Sound player to release.</param>
        public void ReleaseSoundPlayer(SoundPlayer soundPlayer)
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            if (soundPlayer == null)
                throw new ArgumentNullException(nameof(soundPlayer));

            soundPlayers.MarkToRemove(soundPlayer);
        }

        /// <summary>
        /// Plays a sound player.
        /// </summary>
        /// <param name="soundPlayer">Sound player to play.</param>
        public void PlaySoundPlayer(SoundPlayer soundPlayer)
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            if (soundPlayer == null)
                throw new ArgumentNullException(nameof(soundPlayer));

            if (soundPlayer.SourceHandle == null)
                throw new InvalidOperationException("SoundPlayer does not have a valid player handle.");

            if (soundPlayer.BufferHandle == null)
                throw new InvalidOperationException("SoundPlayer does not have a valid sound asset assigned.");

            PlayPlayerCommand playCommand = new PlayPlayerCommand(soundPlayer.SourceHandle, soundPlayer.BufferHandle);

            audioBackend.SendCommand(playCommand);
        }

        /// <summary>
        /// Stops a sound player.
        /// </summary>
        /// <param name="soundPlayer">Sound player to stop.</param>
        public void StopSoundPlayer(SoundPlayer soundPlayer)
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            if (soundPlayer == null)
                throw new ArgumentNullException(nameof(soundPlayer));

            if (soundPlayer.SourceHandle == null)
                throw new InvalidOperationException("SoundPlayer does not have a valid player handle.");

            StopPlayerCommand stopCommand = new StopPlayerCommand(soundPlayer.SourceHandle);

            audioBackend.SendCommand(stopCommand);
        }

        /// <summary>
        /// Pauses a sound player.
        /// </summary>
        /// <param name="soundPlayer">Sound player to pause.</param>
        public void PauseSoundPlayer(SoundPlayer soundPlayer)
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            if (soundPlayer == null)
                throw new ArgumentNullException(nameof(soundPlayer));

            if (soundPlayer.SourceHandle == null)
                throw new InvalidOperationException("SoundPlayer does not have a valid player handle.");

            PausePlayerCommand pauseCommand = new PausePlayerCommand(soundPlayer.SourceHandle);

            audioBackend.SendCommand(pauseCommand);
        }

        /// <summary>
        /// Refreshes the list of audio sources, adding new ones and removing deleted ones.
        /// </summary>
        public void RefreshSources()
        {
            if (disposed)
            {
                return;
            }

            audioSources.Refresh();
        }

        /// <summary>
        /// Refreshes the list of sound players, releasing any that have been removed.
        /// </summary>
        public void RefreshPlayers()
        {
            if (disposed)
            {
                return;
            }

            foreach (SoundPlayer removedSoundPlayer in soundPlayers.PendingRemoves)
            {
                if (removedSoundPlayer.SourceHandle == null)
                    continue;

                ReleaseSourceCommand sourceRelease = new ReleaseSourceCommand(removedSoundPlayer.SourceHandle);

                audioBackend.SendCommand(sourceRelease);
            }

            soundPlayers.Refresh();
        }

        private void ClearAudioSourceManagers()
        {
            ClearAudioSourceManagers(audioSources.Items);
            ClearAudioSourceManagers(audioSources.PendingAdds);
            ClearAudioSourceManagers(audioSources.PendingRemoves);
        }

        private void ClearAudioSourceManagers(IEnumerable<AudioSource> audioSources)
        {
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.AssignAudioManager(null);
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            foreach (SoundPlayer soundplayer in soundPlayers.Items)
            {
                soundPlayers.MarkToRemove(soundplayer);
            }

            foreach (SoundPlayer soundplayer in soundPlayers.PendingAdds)
            {
                soundPlayers.MarkToRemove(soundplayer);
            }

            foreach (SoundPlayer removedSoundPlayer in soundPlayers.PendingRemoves)
            {
                if (removedSoundPlayer.SourceHandle == null)
                    continue;

                ReleaseSourceCommand sourceRelease = new ReleaseSourceCommand(removedSoundPlayer.SourceHandle);

                audioBackend.SendCommand(sourceRelease);
            }

            ClearAudioSourceManagers();

            audioSources.Clear();
            soundPlayers.Clear();
        }
    }
}
