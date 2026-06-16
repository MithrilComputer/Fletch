using Fletch.Audio.Model.AudioData;

namespace Fletch.Assets.Audio.Abstractions
{
    internal interface IAudioDecoder
    {
        bool CanDecode(string path);


        PCMAudioData Decode(string path);
    }
}
