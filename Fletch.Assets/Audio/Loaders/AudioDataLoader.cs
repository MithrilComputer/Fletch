using Fletch.Assets.Audio.Abstractions;
using Fletch.Audio.Model.AudioData;
using System.Text;

namespace Fletch.Assets.Audio.Loaders
{
    internal class AudioDataLoader
    {
        private readonly List<IAudioDecoder> audioDecoders;

        public AudioDataLoader(IEnumerable<IAudioDecoder> audioDecoders)
        {
            this.audioDecoders = audioDecoders.ToList();
        }

        public PCMAudioData Load(string path)
        {
            foreach (IAudioDecoder decoder in audioDecoders)
            {
                if (decoder.CanDecode(path))
                {
                    return decoder.Decode(path);
                }
            }

            throw new NotSupportedException($"No audio decoder found for file: {path}");
        }
    }
}
