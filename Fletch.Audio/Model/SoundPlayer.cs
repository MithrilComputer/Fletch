using Fletch.Audio.Components;

namespace Fletch.Audio.Model
{
    /// <summary>
    /// Represents a controllable sound instance.
    /// </summary>
    public sealed class SoundPlayer : IDisposable
    {
        // TODO: Use dirty detection.

        /// <summary>
        /// Parent audio source.
        /// </summary>
        public AudioSource? ParentAudioSource { get; private set; }

        /// <summary>
        /// Sound volume.
        /// </summary>
        public float Volume { get; set; } = 1f;

        /// <summary>
        /// Sound pitch.
        /// </summary>
        public float Pitch { get; set; } = 1f;

        /// <summary>
        /// Whether the sound loops.
        /// </summary>
        public bool IsLooping { get; set; } = false;

        /// <summary>
        /// Whether playback has been requested.
        /// </summary>
        public bool IsPlaying { get; private set; } = false;

        /// <summary>
        /// Backend source handle.
        /// </summary>
        internal ISoundSourceHandle? SourceHandle { get; private set; }

        /// <summary>
        /// Backend buffer handle.
        /// </summary>
        internal ISoundBufferHandle? BufferHandle { get; private set; }

        /// <summary>
        /// Whether the sound can be played.
        /// </summary>
        private bool IsReadyToPlay => SourceHandle != null && BufferHandle != null;

        private bool preLoadPlayCommanded = false;

        private bool initialized = false;

        private bool disposed = false;

        private readonly List<AudioEffect> effects = new List<AudioEffect>();

        /// <summary>
        /// Creates a sound player.
        /// </summary>
        internal SoundPlayer() { }

        /// <summary>
        /// Initializes the sound player with its parent source.
        /// </summary>
        /// <param name="parentSource">Parent audio source.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="parentSource"/> is null.
        /// </exception>
        internal void Initialize(AudioSource parentSource)
        {
            ParentAudioSource = parentSource ?? throw new ArgumentNullException(nameof(parentSource));

            initialized = true;
        }

        /// <summary>
        /// Assigns the backend sound buffer.
        /// </summary>
        /// <param name="bufferHandle">Sound buffer handle.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="bufferHandle"/> is null.
        /// </exception>
        internal void AssignSoundBuffer(ISoundBufferHandle bufferHandle)
        {
            BufferHandle = bufferHandle ?? throw new ArgumentNullException(nameof(bufferHandle));

            if (preLoadPlayCommanded)
            {
                PlaySound();
            }
        }

        /// <summary>
        /// Assigns the backend sound source.
        /// </summary>
        /// <param name="sourceHandle">Sound source handle.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="sourceHandle"/> is null.
        /// </exception>
        internal void AssignSoundSource(ISoundSourceHandle sourceHandle)
        {
            SourceHandle = sourceHandle ?? throw new ArgumentNullException(nameof(sourceHandle));

            if (preLoadPlayCommanded)
            {
                PlaySound();
            }
        }

        /// <summary>
        /// Plays the sound.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Thrown when this sound player is disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when this sound player is not initialized.
        /// </exception>
        public void PlaySound()
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            CheckInitialized();

            if (!IsReadyToPlay)
            {
                preLoadPlayCommanded = true;
                return;
            }

            preLoadPlayCommanded = false;

            ParentAudioSource?.PlaySoundPlayer(this);

            IsPlaying = true;
        }

        /// <summary>
        /// Stops the sound.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Thrown when this sound player is disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when this sound player is not initialized.
        /// </exception>
        public void StopSound()
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            CheckInitialized();

            if (!IsReadyToPlay)
            {
                preLoadPlayCommanded = false;
                IsPlaying = false;
                return;
            }

            ParentAudioSource?.StopSoundPlayer(this);

            IsPlaying = false;
        }

        /// <summary>
        /// Pauses the sound.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Thrown when this sound player is disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when this sound player is not initialized.
        /// </exception>
        public void PauseSound()
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            CheckInitialized();

            if (!IsReadyToPlay)
            {
                preLoadPlayCommanded = false;
                IsPlaying = false;
                return;
            }

            ParentAudioSource?.PauseSoundPlayer(this);

            IsPlaying = false;
        }

        /*
        /// <summary>
        /// Adds an audio effect.
        /// </summary>
        /// <typeparam name="T">Audio effect type.</typeparam>
        /// <returns>Created audio effect.</returns>
        public AudioEffect AddAudioEffect<T>() where T : AudioEffect
        {
            CheckInitialized();

            throw new NotImplementedException();
        }
        */

        /// <summary>
        /// Gets audio effects.
        /// </summary>
        /// <returns>Audio effects.</returns>
        internal IReadOnlyList<AudioEffect> GetAudioEffects()
        {
            return effects;
        }

        /// <summary>
        /// Checks whether the sound player is initialized.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when this sound player is not initialized.
        /// </exception>
        private void CheckInitialized()
        {
            if (!initialized || ParentAudioSource == null)
            {
                throw new InvalidOperationException("SoundPlayer must be initialized with a parent before use.");
            }
        }

        /// <summary>
        /// Releases this sound player.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            if (ParentAudioSource == null)
            {
                return;
            }

            ParentAudioSource.ReleaseSoundPlayer(this);
        }
    }
}