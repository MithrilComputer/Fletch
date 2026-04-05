using Fletch.Audio.Model;

namespace Fletch.Audio.Silk.NET.OpenAL.Model
{
    internal class OpenALSourceHandle : ISoundBufferHandle
    {
        public uint Id { get; }

        public OpenALSourceHandle(uint id)
        {
            Id = id;
        }
    }
}
