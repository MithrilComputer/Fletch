using Fletch.Audio.Components;
using Fletch.Core.Diagnostics;

namespace Fletch.Audio.Model
{
    public sealed class SoundPlayer : IDisposable
    {
        //TODO Use Dirty detection!

        public AudioSource? ParentAudioSource { get; private set; } 


        public float Volume { get; set; } = 1f;

        public float Pitch { get; set; } = 1f;

        public bool IsLooping { get; set; } = false;

        public bool IsPlaying { get; private set; } = false;


        internal ISoundSourceHandle? SourceHandle { get; private set; }

        internal ISoundBufferHandle? BufferHandle { get; private set; }


        private bool SoundHandleAssigned => BufferHandle != null;


        private bool preLoadPlayCommanded = false;

        private bool initialized = false;

        
        private readonly List<AudioEffect> effects = new List<AudioEffect>();

        private readonly IFletchContextLogger<SoundPlayer> logger;

        internal SoundPlayer(IFletchContextLogger<SoundPlayer> logger)
        {

            this.logger = logger;

        }

        /// <summary>
        /// Called by the parent AudioSource when the SoundPlayer is created. This allows the SoundPlayer to have a reference to its parent AudioSource, which it can use to trigger playback and apply effects through the audio system.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        internal void Initialize(AudioSource parentSource)
        {
            ParentAudioSource = parentSource ?? throw new ArgumentNullException(nameof(parentSource));

            initialized = true;
        }

        /// <summary>
        /// Called after the audio backend has loaded the sound asset and assigned it an ISoundHandle. This allows the SoundPlayer to know when it's ready to play the sound, and to trigger playback if PlaySound was called before the asset was loaded.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        internal void AssignSoundBuffer(ISoundBufferHandle bufferHandle)
        {
            
            BufferHandle = bufferHandle ?? throw new ArgumentNullException(nameof(bufferHandle));

            if (preLoadPlayCommanded)
            {
                PlaySound();
            }

            logger.Log("Buffer Assigned!");
        }

        /// <summary>
        /// Called after the audio backend has loaded the sound asset and assigned it an ISoundHandle. This allows the SoundPlayer to know when it's ready to play the sound, and to trigger playback if PlaySound was called before the asset was loaded.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        internal void AssignSoundSource(ISoundSourceHandle sourceHandle)
        {
            SourceHandle = sourceHandle ?? throw new ArgumentNullException(nameof(sourceHandle));

            if (preLoadPlayCommanded)
            {
                PlaySound();
            }

            logger.Log("Source Assigned!");
        }

        /// <summary>
        /// Initiates playback of the sound, handling preloading if necessary.
        /// </summary>
        public void PlaySound()
        {
            CheckInitialized();
            
            if (!SoundHandleAssigned)
            {
                if(!preLoadPlayCommanded)
                {
                    preLoadPlayCommanded = true;
                }

                return;
            }

            logger.Log("Playing sound");

            ParentAudioSource?.PlaySoundPlayer(this);

            IsPlaying = true;
        }

        /// <summary>
        /// Stops playback of the current sound and updates the playing state.
        /// </summary>
        public void StopSound()
        {
            CheckInitialized();

            if (!SoundHandleAssigned)
            {
                preLoadPlayCommanded = false;
                return;
            }

            ParentAudioSource?.StopSoundPlayer(this);

            IsPlaying = false;
        }

        /// <summary>
        /// Pauses the currently playing sound associated with this instance.
        /// </summary>
        public void PauseSound()
        {
            CheckInitialized();

            if (!SoundHandleAssigned)
            {
                preLoadPlayCommanded = false;
                return;
            }

            ParentAudioSource?.PauseSoundPlayer(this);

            IsPlaying = false;
        }

        /* TODO
        /// <summary>
        /// Creates and adds a new audio effect of the specified type to the audio source.
        /// </summary>
        /// <typeparam name="T">The type of AudioEffect to add.</typeparam>
        /// <returns>The newly created AudioEffect instance.</returns>
        public AudioEffect AddAudioEffect<T>() where T : AudioEffect
        {
            CheckInitialized();

            // uses the singleton IAudioEffectFactory to create an instance of the requested AudioEffect type
            // adds it to the list of effects, and returns it.

            // AudioEffect constructor or whatever

            //ParentAudioSource?.PassAddEffectToSystem(this, null); // replace null with the actual effect instance


            throw new NotImplementedException();
        }
        */

        /// <summary>
        /// Retrieves the collection of audio effects.
        /// </summary>
        /// <returns>A read-only list of AudioEffect objects.</returns>
        internal IReadOnlyList<AudioEffect> GetAudioEffects()
        {
            return effects;
        }

        private void CheckInitialized()
        {
            if (!initialized || ParentAudioSource == null)
            {
                throw new InvalidOperationException("SoundPlayer must be initialized with a parent before use.");
            }
        }

        public void Dispose()
        {
            if (ParentAudioSource == null)
            {
                return;
            }

            ParentAudioSource?.ReleaseSoundPlayer(this);
        }
    }
}
