using Fletch.Audio.Abstractions.Backend;
using Fletch.Audio.Model;
using Fletch.Audio.Model.SoundListenerCommands;
using Fletch.Audio.Model.SoundPlayerCommands;

namespace Fletch.Audio.Silk.NET.OpenAL.Backend
{
    internal sealed class OpenALAudioBackend : IAudioBackend, IDisposable
    {
        public OpenALAudioBackend()
        {

        }

        public void Initialize()
        {

        }

        public ISoundBufferHandle AcquireSound(string localSoundPath)
        {
            throw new NotImplementedException();
        }

        public void ReleaseSound(ISoundBufferHandle soundHandle)
        {
            throw new NotImplementedException();
        }

        public ISoundSourceHandle CreateSoundPlayer()
        {
            throw new NotImplementedException();
        }

        public void DestroySoundPlayer(ISoundSourceHandle soundPlayerHandle)
        {
            throw new NotImplementedException();
        }

        public void SendSoundPlayerCommand(SoundPlayerCommand command)
        {
            throw new NotImplementedException();
        }

        public void SendSoundListenerCommand(SoundListenerCommand command)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }
    }
}
