using Fletch.Audio.Model;

namespace Fletch.Audio.Abstractions.Factories
{
    internal interface ISoundHandleFactory
    {

        public ISoundHandle GetSoundHandle(string soundKey);

    }
}
