using Fletch.Assets.Audio.Loaders;
using Fletch.Audio.Abstractions.Assets;
using Fletch.Audio.Model.AudioData;

namespace Fletch.Assets.Audio.Providers
{
    internal class AudioAssetProvider : IAudioAssetProvider
    {
        //TODO Make this multi threaded

        private readonly AudioDataLoader audioLoader; 

        public AudioAssetProvider(AudioDataLoader audioLoader)
        {
            this.audioLoader = audioLoader;
        }

        public PCMAudioData LoadAudioData(string key)
        {
            return audioLoader.Load(key);
        }
    }
}
