using Fletch.Audio.Abstractions.Assets;
using Fletch.Audio.Abstractions.Backend;
using Fletch.Audio.Model;
using Fletch.Audio.Silk.NET.OpenAL.Natives;
using Fletch.Core.Diagnostics;

namespace Fletch.Audio.Silk.NET.OpenAL.Backend
{
    internal sealed class OpenALAudioBackend : IAudioBackend, IDisposable
    {
        private readonly OpenALManager openALManager;

        public OpenALAudioBackend(IAudioAssetProvider audioAssetProvider, IFletchContextLogger<OpenALManager> ContextManagerLogger)
        {
            openALManager = new OpenALManager(audioAssetProvider, ContextManagerLogger);
            openALManager.Start();
        }

        public void SendCommand(AudioCommand command)
        {
            openALManager.QueueCommand(command);
        }

        public void Dispose()
        {
            openALManager.Dispose();
        }
    }
}
