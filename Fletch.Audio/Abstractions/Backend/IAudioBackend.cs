using Fletch.Audio.Model;

namespace Fletch.Audio.Abstractions.Backend
{
    /// <summary>
    /// Defines the contract for an audio backend, providing methods to manage sound resources, sound players, audio
    /// commands, and effects.
    /// </summary>
    internal interface IAudioBackend
    {
        /// <summary>
        /// Sends an audio command to the backend for processing. This method is responsible for handling various audio commands, such as playing sounds, updating listener properties, and applying audio effects.
        /// </summary>
        /// <param name="command">The audio command to be processed by the backend.</param>
        void SendCommand(AudioCommand command);
    }
}
