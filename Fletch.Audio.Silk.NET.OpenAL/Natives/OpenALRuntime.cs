using Fletch.Audio.Abstractions.Assets;
using Fletch.Audio.Model;
using Fletch.Audio.Model.SoundListenerCommands;
using Fletch.Audio.Model.SoundPlayerCommands;
using Fletch.Audio.Model.SoundPlayerCommands.Buffer;
using Fletch.Audio.Model.SoundPlayerCommands.Source;
using Fletch.Audio.Silk.NET.OpenAL.Model;
using Silk.NET.OpenAL;

namespace Fletch.Audio.Silk.NET.OpenAL.Natives
{
    internal sealed class OpenALRuntime : IDisposable
    {
        private readonly AL al;

        private const int SourcePoolSize = 64; // TODO replace later with a configurable value, and dynamic pool management

        private readonly Queue<uint> sourcePool = new Queue<uint>();

        private readonly Dictionary<OpenALSourceHandle, uint> sourceHandles = new Dictionary<OpenALSourceHandle, uint>();
        private readonly Dictionary<OpenALSoundBufferHandle, uint> bufferHandles = new Dictionary<OpenALSoundBufferHandle, uint>();

        private readonly IAudioAssetProvider audioAssetProvider;

        private bool isDisposed = false;

        public OpenALRuntime(AL al, IAudioAssetProvider audioAssetProvider)
        {
            this.al = al;

            this.audioAssetProvider = audioAssetProvider;

            for (int i = 0; i < SourcePoolSize; i++)
            {
                uint sourceId = al.GenSource();
                sourcePool.Enqueue(sourceId);
            }
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
            switch (command)
            {
                case RequestNewSourceCommand requestNewSourceCommand:

                    break;

                case RequestNewBufferHandleCommand requestNewBufferHandleCommand:

                    break;

                case PlayPlayerCommand playPlayerCommand:
                    
                    break;
            }
        }

        private void HandleListenerCommand(SoundListenerCommand command)
        {
            switch (command)
            {
                
            }
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
