using Fletch.Audio.Model.AudioData;

namespace Fletch.Audio.Abstractions.Assets
{
    internal interface IAudioAssetProvider
    {
        PCMAudioData LoadAudioData(string key);
    }
}
