using Fletch.Audio.Abstractions.Backend;
using Fletch.Audio.Components;
using Fletch.Audio.Model;
using Fletch.Core.LifeCycle;
using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Model;
using Fletch.Engine.Systems;
using Fletch.Engine.Systems.Scenes;

namespace Fletch.Audio.Systems
{
    /// <summary>
    /// The central system for managing scene audio sources, listeners, and sound players.
    /// </summary>
    internal sealed class AudioManagementSystem : SceneSubsystem, IUpdateable, IDisposable
    {
        private readonly IAudioBackend audioBackend;

        private Scene? scene;

        private readonly AudioPlaybackComponentManager audioPlaybackComponentManager;

        private readonly AudioListenerManager audioListenerManager;

        private bool disposed;

        /// <summary>
        /// Creates an audio management system.
        /// </summary>
        /// <param name="audioBackend">Audio backend.</param>
        public AudioManagementSystem(IAudioBackend audioBackend)
        {
            this.audioBackend = audioBackend;

            audioPlaybackComponentManager = new AudioPlaybackComponentManager(audioBackend);

            audioListenerManager = new AudioListenerManager(audioBackend);
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

            scene.AddSystemComponentRegistration(typeof(AudioSource), OnAudioSourceChange);
            scene.AddSystemComponentRegistration(typeof(AudioListener), OnAudioListenerChange);

            this.scene = scene;
        }

        /// <summary>
        /// Handles audio source component changes.
        /// </summary>
        /// <param name="component">Changed component.</param>
        /// <param name="changeType">Component change type.</param>
        private void OnAudioSourceChange(Component component, ComponentChangeType changeType)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            if (component is not AudioSource audioSource)
                throw new InvalidOperationException($"Expected component of type {typeof(AudioSource).FullName}, but received {component.GetType().FullName}.");

            switch (changeType)
            {
                case ComponentChangeType.Added:
                    audioPlaybackComponentManager.AddComponent(audioSource);
                    audioSource.AssignAudioManager(this);
                    break;

                case ComponentChangeType.Removed:
                    audioPlaybackComponentManager.RemoveComponent(audioSource);
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
        private void OnAudioListenerChange(Component component, ComponentChangeType changeType)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            if (component is not AudioListener audioListener)
                throw new InvalidOperationException($"Expected component of type {typeof(AudioListener).FullName}, but received {component.GetType().FullName}.");

            switch (changeType)
            {
                case ComponentChangeType.Added:
                    audioListenerManager.AddListener(audioListener);
                    break;

                case ComponentChangeType.Removed:
                    audioListenerManager.RemoveListener(audioListener);
                    break;

                default: break;
            }
        }

        /// <summary>
        /// Updates audio state.
        /// </summary>
        /// <param name="deltaTime">Frame delta time.</param>
        public void Update(FrameTime deltaTime)
        {
            if (disposed)
            {
                return;
            }

            audioPlaybackComponentManager.RefreshSources();

            audioPlaybackComponentManager.RefreshPlayers();

            audioPlaybackComponentManager.Update();

            audioListenerManager.Update();
        }

        /// <summary>
        /// Requests a new sound player.
        /// </summary>
        /// <param name="soundKey">Sound asset key.</param>
        /// <returns>New sound player.</returns>
        public SoundPlayer RequestNewSoundPlayer(string soundKey)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(typeof(AudioManagementSystem).Name);
            }

            return audioPlaybackComponentManager.RequestNewSoundPlayer(soundKey);
        }

        /// <summary>
        /// Releases a sound player.
        /// </summary>
        /// <param name="soundPlayer">Sound player to release.</param>
        public void ReleaseSoundPlayer(SoundPlayer soundPlayer)
        {
            if (disposed)
            {
                return;
            }

            audioPlaybackComponentManager.ReleaseSoundPlayer(soundPlayer);
        }

        /// <summary>
        /// Plays a sound player.
        /// </summary>
        /// <param name="soundPlayer">Sound player to play.</param>
        public void PlaySoundPlayer(SoundPlayer soundPlayer)
        {
            if (disposed)
            {
                return;
            }

            audioPlaybackComponentManager.PlaySoundPlayer(soundPlayer);
        }

        /// <summary>
        /// Stops a sound player.
        /// </summary>
        /// <param name="soundPlayer">Sound player to stop.</param>
        public void StopSoundPlayer(SoundPlayer soundPlayer)
        {
            if (disposed)
            {
                return;
            }

            audioPlaybackComponentManager.StopSoundPlayer(soundPlayer);
        }

        /// <summary>
        /// Pauses a sound player.
        /// </summary>
        /// <param name="soundPlayer">Sound player to pause.</param>
        public void PauseSoundPlayer(SoundPlayer soundPlayer)
        {
            if (disposed)
            {
                return;
            }

            audioPlaybackComponentManager.PauseSoundPlayer(soundPlayer);
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
                scene.RemoveSystemComponentRegistration(typeof(AudioSource), OnAudioSourceChange);
                scene.RemoveSystemComponentRegistration(typeof(AudioListener), OnAudioListenerChange);
            }

            disposed = true;

            audioListenerManager.Dispose();

            audioPlaybackComponentManager.Dispose();
        }
    }
}