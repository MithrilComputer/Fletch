using Fletch.Audio.Model;
using Fletch.Audio.Systems;
using Fletch.Engine.Components;

namespace Fletch.Audio.Components
{
    public sealed class AudioSource : GameObjectComponent, IDisposable
    {
        // TODO: Add dirty bools for audio updates.

        private readonly List<SoundPlayer> soundPlayers = new List<SoundPlayer>();

        private AudioManagementSystem? audioManagementSystem;

        private bool disposed = false;

        private bool Assigned => audioManagementSystem != null;

        public IReadOnlyList<SoundPlayer> SoundPlayers => soundPlayers;

        internal AudioSource() { }

        internal void AssignAudioManager(AudioManagementSystem? audioManagementSystem)
        {
            this.audioManagementSystem = audioManagementSystem;
        }

        private void ReadyCheck()
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            if (!Assigned)
            {
                throw new InvalidOperationException("AudioSource is not assigned to an audio management system.");
            }
        }

        /// <summary>
        /// Attempts to create and initialize a sound player.
        /// </summary>
        /// <param name="soundKey">Sound asset key.</param>
        /// <param name="soundPlayer">Created sound player.</param>
        /// <returns>True if a sound player was created.</returns>
        public bool TryCreateSoundPlayer(string soundKey, out SoundPlayer? soundPlayer)
        {
            ReadyCheck();

            soundPlayer = audioManagementSystem!.RequestNewSoundPlayer(soundKey);

            if (soundPlayer == null)
            {
                return false;
            }

            soundPlayers.Add(soundPlayer);
            soundPlayer.Initialize(this);

            return true;
        }

        /// <summary>
        /// Releases a sound player owned by this audio source.
        /// </summary>
        /// <param name="soundPlayer">Sound player to release.</param>
        public void ReleaseSoundPlayer(SoundPlayer soundPlayer)
        {
            ReadyCheck();

            if (soundPlayer == null)
            {
                throw new ArgumentNullException(nameof(soundPlayer));
            }

            if (!soundPlayers.Contains(soundPlayer))
            {
                throw new InvalidOperationException("The specified SoundPlayer does not belong to this AudioSource.");
            }

            if (soundPlayer.IsPlaying)
            {
                StopSoundPlayer(soundPlayer);
            }

            soundPlayers.Remove(soundPlayer);

            audioManagementSystem!.ReleaseSoundPlayer(soundPlayer);
        }

        /// <summary>
        /// Gets sound players owned by this audio source.
        /// </summary>
        /// <returns>Sound players.</returns>
        public IReadOnlyList<SoundPlayer> GetSoundPlayers()
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            return soundPlayers;
        }

        internal void PlaySoundPlayer(SoundPlayer player)
        {
            ReadyCheck();

            audioManagementSystem!.PlaySoundPlayer(player);
        }

        internal void StopSoundPlayer(SoundPlayer player)
        {
            ReadyCheck();

            audioManagementSystem!.StopSoundPlayer(player);
        }

        internal void PauseSoundPlayer(SoundPlayer player)
        {
            ReadyCheck();

            audioManagementSystem!.PauseSoundPlayer(player);
        }

        /*
        internal void AddSoundPlayerEffect(SoundPlayer player, AudioEffect effect)
        {
            audioManagementSystem!.AddSoundPlayerEffect(player, effect);
        }

        internal void RemoveSoundPlayerEffect(SoundPlayer player, AudioEffect effect)
        {
            audioManagementSystem!.RemoveSoundPlayerEffect(player, effect);
        }
        */

        /// <summary>
        /// Releases this audio source and its sound players.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            foreach (SoundPlayer soundPlayer in soundPlayers.ToArray())
            {
                soundPlayer.Dispose();
            }

            soundPlayers.Clear();

            disposed = true;
            audioManagementSystem = null;
        }
    }
}