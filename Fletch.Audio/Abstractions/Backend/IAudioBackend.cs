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
