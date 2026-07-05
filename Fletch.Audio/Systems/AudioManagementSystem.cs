using Fletch.Audio.Abstractions.Backend;
using Fletch.Audio.Components;
using Fletch.Audio.Factories.SoundPlayers;
using Fletch.Audio.Model;
using Fletch.Audio.Model.SoundListenerCommands;
using Fletch.Audio.Model.SoundPlayerCommands;
using Fletch.Audio.Model.SoundPlayerCommands.Source;
using Fletch.Core.Components.Update;
using Fletch.Engine.Components;
using Fletch.Engine.Model;
using Fletch.Engine.Scenes;
using Fletch.Engine.Systems;

using System.Numerics;

namespace Fletch.Audio.Systems
{
    /// <summary>
    /// Manages scene audio sources, listeners, and sound players.
    /// </summary>
    internal sealed class AudioManagementSystem : SceneSubsystem, IUpdateable, IDisposable
    {
        private readonly IAudioBackend audioBackend;

        private readonly TrackedSet<AudioListener> audioListeners = new TrackedSet<AudioListener>();

        private readonly TrackedSet<AudioSource> audioSources = new TrackedSet<AudioSource>();

        private readonly TrackedSet<SoundPlayer> soundPlayers = new TrackedSet<SoundPlayer>();

        private Scene? scene;

        private AudioListener? activeListener = null;

        private readonly SoundPlayerFactory soundPlayerFactory;

        private bool disposed;

        /// <summary>
        /// Creates an audio management system.
        /// </summary>
        /// <param name="audioBackend">Audio backend.</param>
        public AudioManagementSystem(IAudioBackend audioBackend)
        {
            this.audioBackend = audioBackend;

            soundPlayerFactory = new SoundPlayerFactory(audioBackend);
        }

        /// <summary>
        /// Registers audio source and listener component change handlers with the scene.
        /// </summary>
        /// <param name="scene">The scene to attach the audio system to.</param>
        public override void AttachToScene(Scene scene)
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            base.AttachToScene(scene);

            scene.SystemManager.RegisterSystem(this, SystemExecutionOrder.Simulation, 0);

            scene.AddSystemComponentRegistration(typeof(AudioSource), (b, c) => OnAudioSourceChange(b, c));
            scene.AddSystemComponentRegistration(typeof(AudioListener), (b, c) => OnAudioListenerChange(b, c));

            this.scene = scene;
        }

        /// <summary>
        /// Handles audio source component changes.
        /// </summary>
        /// <param name="component">Changed component.</param>
        /// <param name="changeType">Component change type.</param>
        private void OnAudioSourceChange(GameObjectComponent component, ComponentChangeType changeType)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            if (component is not AudioSource audioSource)
                throw new InvalidOperationException($"Expected component of type {typeof(AudioSource).FullName}, but received {component.GetType().FullName}.");

            switch (changeType)
            {
                case ComponentChangeType.Added:
                    audioSources.MarkToAdd(audioSource);
                    audioSource.AssignAudioManager(this);
                    break;

                case ComponentChangeType.Removed:
                    audioSources.MarkToRemove(audioSource);
                    audioSource.AssignAudioManager(null);
                    break;

                default: break;
            }
        }

        /// <summary>
        /// Handles audio listener component changes.
        /// </summary>
        /// <param name="component">Changed component.</param>
        /// <param name="changeType">Component change type.</param>
        private void OnAudioListenerChange(GameObjectComponent component, ComponentChangeType changeType)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            if (component is not AudioListener audioListener)
                throw new InvalidOperationException($"Expected component of type {typeof(AudioListener).FullName}, but received {component.GetType().FullName}.");

            switch (changeType)
            {
                case ComponentChangeType.Added:
                    audioListeners.MarkToAdd(audioListener);
                    break;

                case ComponentChangeType.Removed:
                    audioListeners.MarkToRemove(audioListener);
                    break;

                default: break;
            }

            audioListeners.Sort(ListenerCompare);
        }

        /// <summary>
        /// Updates audio state.
        /// </summary>
        /// <param name="deltaTime">Frame delta time.</param>
        public void Update(float deltaTime)
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
        /// Updates sound players.
        /// </summary>
        private void UpdatePlayers()
        {
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

                Vector2 position = soundPlayer.ParentAudioSource.GameObject.Transform.WorldPosition;

                SetPlayerPositionCommand setPositionCommand = new SetPlayerPositionCommand(soundPlayer.SourceHandle, position);

                SetPlayerPitchCommand pitchCommand = new SetPlayerPitchCommand(soundPlayer.SourceHandle, soundPlayer.Pitch);

                SetPlayerLoopingCommand loopingCommand = new SetPlayerLoopingCommand(soundPlayer.SourceHandle, soundPlayer.IsLooping);

                SetPlayerVolumeCommand volumeCommand = new SetPlayerVolumeCommand(soundPlayer.SourceHandle, soundPlayer.Volume);

                audioBackend.SendCommand(setPositionCommand); //TODO Use Dirty detection to avoid sending this every frame.
                audioBackend.SendCommand(pitchCommand); //TODO Use Dirty detection to avoid sending this every frame.
                audioBackend.SendCommand(loopingCommand); //TODO Use Dirty detection to avoid sending this every frame.
                audioBackend.SendCommand(volumeCommand); //TODO Use Dirty detection to avoid sending this every frame.
            }
        }

        /// <summary>
        /// Updates the active listener.
        /// </summary>
        private void UpdateListeners()
        {
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

        /// <summary>
        /// Clears audio manager references from tracked audio sources.
        /// </summary>
        private void ClearAudioSourceManagers()
        {
            ClearAudioSourceManagers(audioSources.Items);
            ClearAudioSourceManagers(audioSources.PendingAdds);
            ClearAudioSourceManagers(audioSources.PendingRemoves);
        }

        /// <summary>
        /// Clears audio manager references from audio sources.
        /// </summary>
        /// <param name="audioSources">Audio sources to clear.</param>
        private void ClearAudioSourceManagers(IEnumerable<AudioSource> audioSources)
        {
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.AssignAudioManager(null);
            }
        }

        /// <summary>
        /// Releases audio management system resources.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            if (scene != null)
            {
                scene.RemoveSystemComponentRegistration(typeof(AudioSource), (b, c) => OnAudioSourceChange(b, c));
                scene.RemoveSystemComponentRegistration(typeof(AudioListener), (b, c) => OnAudioListenerChange(b, c));
            }

            disposed = true;

            ClearAudioSourceManagers();

            activeListener = null;

            audioListeners.Clear();
            audioSources.Clear();
            soundPlayers.Clear();
        }
    }
}