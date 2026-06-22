using Fletch.Audio.Model;

namespace Fletch.Audio.Silk.NET.OpenAL.Model
{
    internal class OpenALSourceHandle : ISoundSourceHandle
    {
        public uint Id { get; }

        public OpenALSourceHandle(uint id)
        {
            Id = id;
        }
    }
}
