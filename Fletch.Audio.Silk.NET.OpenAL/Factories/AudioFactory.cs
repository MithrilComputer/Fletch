using Silk.NET.OpenAL;

namespace Fletch.Audio.Silk.NET.OpenAL.Factories
{
    internal sealed class AudioFactory
    {
        private readonly ALContext aLContext;

        public AudioFactory(ALContext aLContext)
        {
            this.aLContext = aLContext;
        }

        public void LoadAudioFile()
        {

        }
    }
}
