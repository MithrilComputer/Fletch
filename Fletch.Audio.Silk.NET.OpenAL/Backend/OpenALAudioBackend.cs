using Fletch.Audio.Abstractions.Assets;
using Fletch.Audio.Abstractions.Backend;
using Fletch.Audio.Model;
using Fletch.Audio.Silk.NET.OpenAL.Natives;

namespace Fletch.Audio.Silk.NET.OpenAL.Backend
{
    internal sealed class OpenALAudioBackend : IAudioBackend, IDisposable
    {
        private readonly OpenALManager openALManager;

        public OpenALAudioBackend(IAudioAssetProvider audioAssetProvider)
        {
            openALManager = new OpenALManager(audioAssetProvider);
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
