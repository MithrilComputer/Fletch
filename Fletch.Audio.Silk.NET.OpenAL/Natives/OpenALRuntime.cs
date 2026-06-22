using Fletch.Audio.Abstractions.Assets;
using Fletch.Audio.Model;
using Fletch.Audio.Model.AudioData;
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
        private readonly Dictionary<OpenALBufferHandle, uint> bufferHandles = new Dictionary<OpenALBufferHandle, uint>();

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
                case RequestNewSourceCommand newSourceRequest: // TODO Make a fallback for if the pool is drained, simple for testing perpouses for now

                    newSourceRequest.Deconstruct(out TaskCompletionSource<ISoundSourceHandle> sourceHandleRequest);

                    uint newSourceID = sourcePool.Dequeue();

                    sourceHandleRequest.SetResult(new OpenALSourceHandle(newSourceID));

                    break;

                case RequestNewBufferHandleCommand requestNewBufferHandleCommand:

                    requestNewBufferHandleCommand.Deconstruct(out string key,out TaskCompletionSource<ISoundBufferHandle> bufferHandleRequest);

                    PCMAudioData PCMData = audioAssetProvider.LoadAudioData(key);

                    OpenALBufferHandle bufferRequestHandle = new OpenALBufferHandle(al.GenBuffer());

                    UploadBuffer(bufferRequestHandle, PCMData); // TODO This is unsafe, please deal with ASAP, testing for now

                    bufferHandleRequest.SetResult(bufferRequestHandle);

                    break;

                case PlayPlayerCommand playPlayerCommand:

                    playPlayerCommand.Deconstruct(out ISoundSourceHandle playSourceHandle, out ISoundBufferHandle playBufferHandle);

                    if (playSourceHandle is not OpenALSourceHandle alPlaySourceHandle)
                        throw new InvalidOperationException();

                    if (playBufferHandle is not OpenALBufferHandle alPlayBufferHandle)
                        throw new InvalidOperationException();

                    al.SetSourceProperty(
                        alPlaySourceHandle.Id,
                        SourceInteger.Buffer,
                        alPlayBufferHandle.Id);

                    al.SourcePlay(alPlaySourceHandle.Id);

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

        private unsafe void UploadBuffer(OpenALBufferHandle bufferHandle, PCMAudioData pcmData)
        {
            fixed (byte* pcmDataPointer = pcmData.Data)
            {
                al.BufferData(
                    bufferHandle.Id,
                    GetOpenALBufferFormat(pcmData),
                    pcmDataPointer,
                    pcmData.ByteCount,
                    (int)pcmData.SampleRate);
            }
        }

        private BufferFormat GetOpenALBufferFormat(PCMAudioData audioData)
        {
            if(audioData.Channels == 1) //Mono
            {
                if(audioData.BitsPerSample == 8)
                {
                    return BufferFormat.Mono8;
                }

                if(audioData.BitsPerSample == 16)
                {
                    return BufferFormat.Mono16;
                }
            }

            if (audioData.Channels == 2) //Stereo
            {
                if (audioData.BitsPerSample == 8)
                {
                    return BufferFormat.Stereo8;
                }

                if (audioData.BitsPerSample == 16)
                {
                    return BufferFormat.Stereo16;
                }
            }

            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }
    }
}
