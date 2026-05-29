using Fletch.Audio.Model;
using Fletch.Audio.Model.SoundListenerCommands;
using Fletch.Audio.Model.SoundPlayerCommands;
using Fletch.Audio.Silk.NET.OpenAL.Model;
using Silk.NET.OpenAL;

namespace Fletch.Audio.Silk.NET.OpenAL.Natives
{
    internal sealed class OpenALRuntime : IDisposable
    {
        private readonly AL al;

        private readonly Queue<uint> sourcePool = new Queue<uint>();

        private readonly Dictionary<OpenALSourceHandle, uint> sourceHandles = new Dictionary<OpenALSourceHandle, uint>();
        private readonly Dictionary<OpenALSoundBufferHandle, uint> bufferHandles = new Dictionary<OpenALSoundBufferHandle, uint>();

        private bool isDisposed = false;

        public OpenALRuntime(AL al)
        {
            this.al = al;
        }

        public void HandleAudioCommand(AudioCommand command)
        {
            ThrowIfDisposed();

            switch (command)
            {
                case SoundPlayerCommand playerCommand:
                    HandlePlayerCommand(playerCommand);
                    break;

                case SoundListenerCommand listenerCommand:
                    HandleListenerCommand(listenerCommand);
                    break;

                default:
                    throw new InvalidOperationException($"Unknown audio command type: {command.GetType().FullName}");
            }
        }

        private void HandlePlayerCommand(SoundPlayerCommand command)
        {
            //TODO
        }

        private void HandleListenerCommand(SoundListenerCommand command)
        {
            //TODO
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(OpenALRuntime));
            }
        }

        public void Dispose()
        {

        }
    }
}
