using Fletch.Audio.Model;
using Fletch.Audio.Model.SoundListenerCommands;
using Fletch.Audio.Model.SoundPlayerCommands;

namespace Fletch.Audio.Abstractions.Backend
{
    /// <summary>
    /// Defines the contract for an audio backend, providing methods to manage sound resources, sound players, audio
    /// commands, and effects.
    /// </summary>
    internal interface IAudioBackend
    {
        /// <summary>
        /// Loads a sound resource for engine use.
        /// This call may block depending on backend and file format.
        /// Higher engine layers are responsible for deciding whether to call it on a worker thread.
        /// </summary>
        /// <param name="localSoundPath">The path to the local sound file.</param>
        /// <returns>An ISoundHandle representing the loaded sound.</returns>
        ISoundBufferHandle AcquireSound(string localSoundPath);

        /// <summary>
        /// Releases resources associated with the specified sound handle.
        /// </summary>
        /// <param name="soundHandle">The sound handle to release.</param>
        void ReleaseSound(ISoundBufferHandle soundHandle);

        /// <summary>
        /// Creates a new sound player handle.
        /// </summary>
        /// <returns>A handle to the newly created sound player.</returns>
        ISoundSourceHandle CreateSoundPlayer();

        /// <summary>
        /// Releases resources associated with the specified sound player.
        /// </summary>
        /// <param name="soundPlayerHandle">The identifier of the sound player to dispose.</param>
        void DestroySoundPlayer(ISoundSourceHandle soundPlayerHandle);

        /// <summary>
        /// Sends a specified audio command to the sound player by the given sound player ID.
        /// </summary>
        /// <param name="command">The audio command to send to the sound player.</param>
        void SendSoundPlayerCommand(SoundPlayerCommand command);

        /// <summary>
        /// Sends a command to the sound listener.
        /// </summary>
        /// <param name="command">The command to be sent to the sound listener.</param>
        void SendSoundListenerCommand(SoundListenerCommand command);
    }
}
