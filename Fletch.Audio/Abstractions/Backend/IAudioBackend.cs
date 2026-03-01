using Fletch.Audio.Abstractions.Factories;
using Fletch.Audio.Model;
using Fletch.Audio.Model.Commands;

namespace Fletch.Audio.Abstractions.Backend
{
    /// <summary>
    /// The IAudioBackend interface defines the contract for audio backends in the Fletch audio system. It provides methods for managing sound players, sending audio commands, and applying audio effects. Implementations of this interface will handle the actual audio processing and playback logic, allowing for flexibility in choosing different audio libraries or platforms while maintaining a consistent API for the rest of the system.
    /// </summary>
    internal interface IAudioBackend
    {
        /// <summary>
        /// Gets the factory used to create sound handle instances.
        /// </summary>
        ISoundHandleFactory SoundHandleFactory { get; }

        /// <summary>
        /// Creates and returns a new sound player instance identifier.
        /// </summary>
        /// <returns>An int representing the ID of the sound player.</returns>
        ISoundPlayerHandle CreateNewSoundPlayer();

        /// <summary>
        /// Releases resources associated with the specified sound player.
        /// </summary>
        /// <param name="soundPlayerID">The identifier of the sound player to dispose.</param>
        void DisposeSoundPlayer(ISoundPlayerHandle soundPlayerID);

        /// <summary>
        /// Sends a specified audio command to the sound player by the given sound player ID.
        /// </summary>
        /// <param name="soundPlayerID">The id of the sound player to receive the command.</param>
        /// <param name="command">The audio command to send to the sound player.</param>
        void SendSoundPlayerCommand(uint soundPlayerID, AudioCommand command);

        /// <summary>
        /// Applies an audio effect to the specified sound player.
        /// </summary>
        /// <param name="soundPlayerID">The unique identifier of the sound player.</param>
        /// <param name="effect">The audio effect to apply.</param>
        void AddEffectToSoundPlayer(uint soundPlayerID, AudioEffect effect);

        /// <summary>
        /// Removes an audio effect from the specified sound player.
        /// </summary>
        /// <param name="soundPlayerID">The unique identifier of the sound player.</param>
        /// <param name="effect">The audio effect to remove.</param>
        bool RemoveEffectFromSoundPlayer(uint soundPlayerID, AudioEffect effect);
    }
}
