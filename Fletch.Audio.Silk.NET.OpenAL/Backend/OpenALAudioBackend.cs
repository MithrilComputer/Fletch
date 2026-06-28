using Fletch.Audio.Abstractions.Assets;
using Fletch.Audio.Abstractions.Backend;
using Fletch.Audio.Model;
using Fletch.Audio.Silk.NET.OpenAL.Natives;
using Fletch.Core.Diagnostics;

namespace Fletch.Audio.Silk.NET.OpenAL.Backend
{
    /// <summary>
    /// OpenAL audio backend.
    /// </summary>
    internal sealed class OpenALAudioBackend : IAudioBackend, IDisposable
    {
        private readonly OpenALManager openALManager;

        /// <summary>
        /// Creates an OpenAL audio backend.
        /// </summary>
        /// <param name="audioAssetProvider">Audio asset provider.</param>
        /// <param name="contextManagerLogger">OpenAL manager logger.</param>
        public OpenALAudioBackend(IAudioAssetProvider audioAssetProvider, IFletchContextLogger<OpenALManager> contextManagerLogger)
        {
            openALManager = new OpenALManager(audioAssetProvider, contextManagerLogger);
            openALManager.Start();
        }

        /// <summary>
        /// Sends an audio command.
        /// </summary>
        /// <param name="command">Audio command.</param>
        public void SendCommand(AudioCommand command)
        {
            openALManager.QueueCommand(command);
        }

        /// <summary>
        /// Releases backend resources.
        /// </summary>
        public void Dispose()
        {
            openALManager.Dispose();
        }
    }
}
