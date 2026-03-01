using Fletch.Audio.Components;

namespace Fletch.Audio.Model
{
    public sealed class SoundPlayer : IDisposable
    {
        public float Volume { get; set; } = 1f;

        public float Pitch { get; set; } = 1f;

        public bool IsLooping { get; set; } = false;

        public bool IsPlaying { get; private set; } = false;


        internal ISoundPlayerHandle PlayerHandle { get; }

        internal ISoundHandle AssetHandle { get; }


        private readonly AudioSource speakerSource;

        private readonly List<AudioEffect> effects = new List<AudioEffect>();


        internal SoundPlayer(AudioSource speakerSource, ISoundHandle assetHandle, ISoundPlayerHandle playerHandle)
        {
            this.speakerSource = speakerSource ?? throw new ArgumentNullException(nameof(speakerSource));

            AssetHandle = assetHandle ?? throw new ArgumentNullException(nameof(assetHandle));

            PlayerHandle = playerHandle ?? throw new ArgumentNullException(nameof(playerHandle));
        }

        public void PlaySound()
        {
            IsPlaying = true;
            // Trigger the sound to play using the audio system, applying volume, pitch, looping, and effects as necessary.
            // Do this through the parent AudioSource
        }

        public void StopSound()
        {
            IsPlaying = false;
            // Trigger the sound to stop using the audio system
            // Do this through the parent AudioSource
        }

        public void PauseSound()
        {
            IsPlaying = false;
            // Trigger the sound to pause using the audio system
            // Do this through the parent AudioSource
        }

        public AudioEffect AddAudioEffect<T>() where T : AudioEffect
        {
            // uses the singleton IAudioEffectFactory to create an instance of the requested AudioEffect type
            // adds it to the list of effects, and returns it.
        }

        internal IReadOnlyList<AudioEffect> GetAudioEffects()
        {
            return effects;
        }

        public void Dispose()
        {
            speakerSource.DestroySoundPlayer(this);
        }
    }
}
