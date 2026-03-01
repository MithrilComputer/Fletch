using Fletch.Audio.Abstractions.Factories;
using Fletch.Audio.Model;
using Fletch.Engine.Components;

namespace Fletch.Audio.Components
{
    public sealed class AudioSource : GameObjectComponent, IDisposable
    {
        private readonly List<SoundPlayer> soundPlayers = new List<SoundPlayer>();
        private readonly ISoundHandleFactory soundHandleFactory;

        internal AudioSource(ISoundHandleFactory soundHandleFactory)
        {
            this.soundHandleFactory = soundHandleFactory ?? throw new ArgumentNullException(nameof(soundHandleFactory));
        }

        public SoundPlayer CreateSoundPlayer(string soundKey)
        {
            ISoundHandle soundHandle = soundHandleFactory.GetSoundHandle(soundKey);

            SoundPlayer soundPlayer = new SoundPlayer(this, soundHandle);

            soundPlayerIndex++;

            soundPlayers.Add(soundPlayer);

            return soundPlayer;
        }

        internal void DestroySoundPlayer(SoundPlayer soundPlayer)
        {
            soundPlayers.Remove(soundPlayer);
        }

        internal IReadOnlyList<SoundPlayer> GetSoundPlayers()
        {
            return soundPlayers;
        }

        public void Dispose()
        {

        }
    }
}
