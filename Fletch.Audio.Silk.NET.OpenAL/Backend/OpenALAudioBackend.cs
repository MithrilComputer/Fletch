using Fletch.Audio.Abstractions.Backend;
using Fletch.Audio.Model;
using Fletch.Audio.Silk.NET.OpenAL.Natives;
using Silk.NET.OpenAL;

namespace Fletch.Audio.Silk.NET.OpenAL.Backend
{
    internal sealed class OpenALAudioBackend : IAudioBackend, IDisposable
    {
        private readonly ALContext alContext;

        private readonly Thread thread;

        public OpenALAudioBackend()
        {
            openALNative = new OpenALNative(); //Starts backend device and context

            alContext = ALContext.GetApi();


        }
        
        public SoundHandle? QueueCommand(AudioCommand command)
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
