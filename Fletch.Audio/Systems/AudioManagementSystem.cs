using Fletch.Audio.Abstractions.Backend;
using Fletch.Audio.Components;
using Fletch.Audio.Factories.SoundPlayers;
using Fletch.Audio.Model;
using Fletch.Audio.Model.SoundListenerCommands;
using Fletch.Audio.Model.SoundPlayerCommands;
using Fletch.Core.Components.Update;
using Fletch.Core.Diagnostics;
using Fletch.Engine.Components;
using Fletch.Engine.Model;
using Fletch.Engine.Scenes;
using Fletch.Engine.Systems;
using System.Diagnostics;
using System.Numerics;

namespace Fletch.Audio.Systems
{
    internal sealed class AudioManagementSystem : SceneSubsystem, IUpdateable
    {
        private readonly IAudioBackend audioBackend;

        private readonly TrackedSet<AudioListener> audioListeners = new TrackedSet<AudioListener>();

        private readonly TrackedSet<AudioSource> audioSources = new TrackedSet<AudioSource>();

        private readonly TrackedSet<SoundPlayer> soundPlayers = new TrackedSet<SoundPlayer>();

        private AudioListener? activeListener = null;

        private readonly SoundPlayerFactory soundPlayerFactory;

        private readonly IFletchLogger logger;

        public AudioManagementSystem(IAudioBackend audioBackend, IFletchLogger logger)
        {
            this.audioBackend = audioBackend;

            this.logger = logger;

            soundPlayerFactory = new SoundPlayerFactory(audioBackend);
        }

        /// <summary>
        /// Registers audio source and listener component change handlers with the scene.
        /// </summary>
        /// <param name="scene">The scene to attach the audio system to.</param>
        public override void AttachToScene(Scene scene)
        {
            base.AttachToScene(scene);

            scene.SystemManager.RegisterSystem(this, SystemExecutionOrder.Simulation, 0);

            scene.AddSystemComponentRegistration(typeof(AudioSource), (b, c) => OnAudioSourceChange(b, c));
            scene.AddSystemComponentRegistration(typeof(AudioListener), (b, c) => OnAudioListenerChange(b, c));
        }

        /// <summary>
        /// Handles changes to an AudioSource component by marking it for addition or removal based on the specified
        /// change type.
        /// </summary>
        /// <param name="component">The component to process as an AudioSource.</param>
        /// <param name="changeType">The type of change applied to the component.</param>
        /// <exception cref="ArgumentNullException">Thrown when the component parameter is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the component is not of type AudioSource.</exception>
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
                    audioSource.AssignAudioManager(this);
                    break;

                default: break;
            }
        }

        /// <summary>
        /// Handles changes to AudioListener components by marking them for addition or removal based on the specified
        /// change type.
        /// </summary>
        /// <param name="component">The component to process as an AudioListener.</param>
        /// <param name="changeType">The type of change applied to the component.</param>
        /// <exception cref="ArgumentNullException">Thrown if the component parameter is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the component is not of type AudioListener.</exception>
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

        public void Update(float deltaTime)
        {
            audioListeners.Refresh();

            audioListeners.Sort(ListenerCompare);

            audioSources.Refresh();

            soundPlayers.Refresh();

            UpdatePlayers();

            UpdateListeners();
        }

        /// <summary>
        /// Creates a new SoundPlayer instance, begins loading the sound asset identified by the specified key, and assigns it to the player once loading is complete.
        /// </summary>
        /// <param name="soundKey">The key identifying the sound asset to load.</param>
        /// <returns>A new SoundPlayer instance if creation succeeds; otherwise, null.</returns>
        public SoundPlayer? RequestNewSoundPlayer(string soundKey)
        {
            SoundPlayer soundPlayer = soundPlayerFactory.Create(soundKey);

            soundPlayers.MarkToAdd(soundPlayer);

            return soundPlayer;
        }

        /// <summary>
        /// Releases the specified SoundPlayer and destroys its associated player handle.
        /// </summary>
        /// <param name="soundPlayer">The SoundPlayer instance to release.</param>
        /// <exception cref="ArgumentNullException">Thrown if soundPlayer is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if soundPlayer does not have a valid player handle.</exception>
        public void ReleaseSoundPlayer(SoundPlayer soundPlayer)
        {
            /*
            if (soundPlayer == null)
                throw new ArgumentNullException(nameof(soundPlayer));

            if (soundPlayer.SourceHandle == null)
                throw new InvalidOperationException("SoundPlayer does not have a valid player handle.");

            soundPlayers.MarkToRemove(soundPlayer);

            audioBackend.DestroySoundPlayer(soundPlayer.SourceHandle);
            */
        }

        /// <summary>
        /// Plays the specified SoundPlayer using the audio backend.
        /// </summary>
        /// <param name="soundPlayer">The SoundPlayer instance to play.</param>
        /// <exception cref="ArgumentNullException">Thrown if soundPlayer is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if soundPlayer does not have a valid player handle or sound asset assigned.</exception>
        public void PlaySoundPlayer(SoundPlayer soundPlayer)
        {
            if (soundPlayer == null)
                throw new ArgumentNullException(nameof(soundPlayer));

            if (soundPlayer.SourceHandle == null)
                throw new InvalidOperationException("SoundPlayer does not have a valid player handle.");

            if (soundPlayer.BufferHandle == null)
                throw new InvalidOperationException("SoundPlayer does not have a valid sound asset assigned.");

            PlayPlayerCommand playCommand = new PlayPlayerCommand(soundPlayer.SourceHandle, soundPlayer.BufferHandle);

            audioBackend.SendCommand(playCommand);
        }

        public void StopSoundPlayer(SoundPlayer soundPlayer)
        {
            if (soundPlayer == null)
                throw new ArgumentNullException(nameof(soundPlayer));

            if (soundPlayer.SourceHandle == null)
                throw new InvalidOperationException("SoundPlayer does not have a valid player handle.");

            StopPlayerCommand stopCommand = new StopPlayerCommand(soundPlayer.SourceHandle);

            audioBackend.SendCommand(stopCommand);
        }

        public void PauseSoundPlayer(SoundPlayer soundPlayer)
        {
            if (soundPlayer == null)
                throw new ArgumentNullException(nameof(soundPlayer));

            if (soundPlayer.SourceHandle == null)
                throw new InvalidOperationException("SoundPlayer does not have a valid player handle.");

            PausePlayerCommand pauseCommand = new PausePlayerCommand(soundPlayer.SourceHandle);

            audioBackend.SendCommand(pauseCommand);
        }

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

        private void UpdateListeners()
        {
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

                //audioBackend.SendCommand(gainCommand); //TODO Use Dirty detection to avoid sending this every frame.
                //audioBackend.SendCommand(positionCommand); //TODO Use Dirty detection to avoid sending this every frame.
            }
            else
            {
                SetListenerGainCommand gainCommand = new SetListenerGainCommand(0f); //TODO Use Dirty detection to avoid sending this every frame.

                //audioBackend.SendCommand(gainCommand); //TODO Use Dirty detection to avoid sending this every frame.
            }
        }

        private static int ListenerCompare(AudioListener a, AudioListener b)
        {
            return a.priority.CompareTo(b.priority);
        }
    }
}