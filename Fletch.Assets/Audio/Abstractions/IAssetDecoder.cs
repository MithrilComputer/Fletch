using Fletch.Audio.Model.AudioData;

namespace Fletch.Assets.Audio.Abstractions
{
    internal interface IAssetDecoder
    {
        bool CanDecode(string path);

        PCMAudioData Decode(string path);
    }
}
