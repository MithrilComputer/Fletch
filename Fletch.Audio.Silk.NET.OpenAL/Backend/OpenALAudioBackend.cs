using Fletch.Audio.Abstractions.Backend;
using Fletch.Audio.Model;
using Silk.NET.OpenAL;

namespace Fletch.Audio.Silk.NET.OpenAL.Backend
{
    internal sealed class OpenALAudioBackend : IAudioBackend, IDisposable
    {
        private readonly ALContext alContext;

        private readonly Thread thread;

        public OpenALAudioBackend()
        {
            alContext = ALContext.GetApi();


        }
        
        public ISoundHandle? QueueCommand(AudioCommand command)
        {

        }

        public void Initialize()
        {

            

        }

        public void Dispose()
        {
            openALNative.Shutdown();
        }
    }
}
