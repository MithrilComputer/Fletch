using Fletch.Audio.Model;

namespace Fletch.Audio.Silk.NET.OpenAL.Model
{
    internal class OpenALSoundBufferHandle : ISoundSourceHandle
    {
        public uint Id { get; }

        public OpenALSoundBufferHandle(uint id)
        {
            Id = id;
        }
    }
}
