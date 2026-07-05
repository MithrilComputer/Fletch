using Fletch.Assets.Audio.Abstractions;
using Fletch.Assets.Audio.Decoders;
using Fletch.Assets.Audio.Loaders;
using Fletch.Audio.Abstractions.Assets;
using Fletch.Audio.Model.AudioData;

namespace Fletch.Assets.Audio.Providers
{
    internal sealed class AudioAssetProvider : IAudioAssetProvider, IDisposable
    {
        //TODO Make this multi threaded

        private readonly AudioDataLoader audioLoader;

        private readonly List<IAudioDecoder> audioDecoders = new List<IAudioDecoder>();

        public AudioAssetProvider()
        {
            WavDecoder wavDecoder = new WavDecoder();

            audioDecoders.Add(wavDecoder);

            audioLoader = new AudioDataLoader(audioDecoders);
        }

        public PCMAudioData LoadAudioData(string key) // TODO Make into an Async task, also handle exceptions
        {
            return audioLoader.Load(key);
        }

        public void Dispose()
        {
            foreach (IAudioDecoder decoder in audioDecoders)
            {
                if (decoder is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            audioDecoders.Clear();
        }
    }
}
