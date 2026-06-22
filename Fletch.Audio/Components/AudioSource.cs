using Fletch.Audio.Model;
using Fletch.Audio.Systems;
using Fletch.Core.Diagnostics;
using Fletch.Engine.Components;

namespace Fletch.Audio.Components
{
    public sealed class AudioSource : GameObjectComponent, IDisposable
    {
        //TODO Add a dirty bool for others to use for updating.

        private readonly List<SoundPlayer> soundPlayers = new List<SoundPlayer>();

        private AudioManagementSystem audioManagementSystem;


        IFletchContextLogger<AudioSource> logger;

        internal AudioSource(IFletchContextLogger<AudioSource> logger)
        {

            this.logger = logger;

        }

        internal void AssignAudioManager(AudioManagementSystem audioManagementSystem)
        {
            this.audioManagementSystem = audioManagementSystem;
        }

        private void ReadyCheck()
        {
            if(audioManagementSystem == null)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Attempts to create and initialize a new SoundPlayer for the specified sound key.
        /// </summary>
        /// <param name="soundKey">The key identifying the sound to create a player for.</param>
        /// <param name="soundPlayer">When the method returns, contains the created SoundPlayer if successful; otherwise, null.</param>
        /// <returns>True if a SoundPlayer was successfully created and initialized; otherwise, false.</returns>
        public bool TryCreateSoundPlayer(string soundKey, out SoundPlayer? soundPlayer)
        {
            ReadyCheck();

            soundPlayer = audioManagementSystem.RequestNewSoundPlayer(soundKey);

            if (soundPlayer == null)
                return false;

            soundPlayers.Add(soundPlayer);

            soundPlayer.Initialize(this);

            return true;
        }

        public void ReleaseSoundPlayer(SoundPlayer soundPlayer)
        {
            ReadyCheck();

            if (!soundPlayers.Contains(soundPlayer))
            {
                throw new InvalidOperationException("The specified SoundPlayer does not belong to this AudioSource.");
            }

            if (soundPlayer.IsPlaying)
            {
                soundPlayer.StopSound();
            }

            if (soundPlayer == null)
            {
                throw new ArgumentNullException(nameof(soundPlayer));
            }

            soundPlayers.Remove(soundPlayer);

            audioManagementSystem.ReleaseSoundPlayer(soundPlayer);
        }

        public IReadOnlyList<SoundPlayer> GetSoundPlayers()
        {
            return soundPlayers;
        }

        internal void PlaySoundPlayer(SoundPlayer player)
        {
            ReadyCheck();

            logger.Log("Playing");

            audioManagementSystem.PlaySoundPlayer(player);
        }

        internal void StopSoundPlayer(SoundPlayer player)
        {
            ReadyCheck();

            audioManagementSystem.StopSoundPlayer(player);
        }

        internal void PauseSoundPlayer(SoundPlayer player)
        {
            ReadyCheck();

            audioManagementSystem.PauseSoundPlayer(player);
        }

        /* No effects just yet, but this is where we would add them to the player and pass them to the
        internal void AddSoundPlayerEffect(SoundPlayer player, AudioEffect effect)
        {
            audioManagementSystem.AddSoundPlayerEffect(player, effect);
        }

        internal void PassRemoveEffectToSystem(SoundPlayer player, AudioEffect effect)
        {
            audioManagementSystem.RemoveSoundPlayerEffect(player, effect);
        }
        */

        public void Dispose()
        {

        }
    }
}
