using Fletch.Audio.Model;

namespace Fletch.Audio.Silk.NET.OpenAL.Model
{
    internal class OpenALBufferHandle : ISoundBufferHandle
    {
        public uint Id { get; }

        public OpenALBufferHandle(uint id)
        {
            Id = id;
        }
    }
}
