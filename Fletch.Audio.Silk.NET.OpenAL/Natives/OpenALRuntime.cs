using Fletch.Audio.Abstractions.Assets;
using Fletch.Audio.Model;
using Fletch.Audio.Model.AudioData;
using Fletch.Audio.Model.SoundListenerCommands;
using Fletch.Audio.Model.SoundPlayerCommands;
using Fletch.Audio.Model.SoundPlayerCommands.Buffer;
using Fletch.Audio.Model.SoundPlayerCommands.Source;
using Fletch.Audio.Silk.NET.OpenAL.Model;
using Silk.NET.OpenAL;
using System.Numerics;

namespace Fletch.Audio.Silk.NET.OpenAL.Natives
{
    internal sealed class OpenALRuntime : IDisposable 
    {
        private readonly AL al;

        private const int SourcePoolSize = 64; // TODO replace later with a configurable value, and dynamic pool management

        private readonly Queue<uint> sourcePool = new Queue<uint>();

        private readonly List<uint> allSources = new List<uint>();

        private readonly Dictionary<string, uint> bufferCache = new Dictionary<string, uint>();

        private readonly Dictionary<uint, string> bufferToKey = new Dictionary<uint, string>();

        /// <summary>
        /// Maps currently used buffers to the sources using them
        /// </summary>
        private readonly Dictionary<uint, List<uint>> bufferToSource;

        /// <summary>
        /// Maps currently used sources to the buffer thats being used
        /// </summary>
        private readonly Dictionary<uint, uint> sourceToBuffer = new Dictionary<uint, uint>();

        private readonly IAudioAssetProvider audioAssetProvider;

        private bool isDisposed = false;

        public OpenALRuntime(AL al, IAudioAssetProvider audioAssetProvider)
        {
            this.al = al;

            this.audioAssetProvider = audioAssetProvider;

            bufferToSource = new Dictionary<uint, List<uint>>();

            for (int i = 0; i < SourcePoolSize; i++)
            {
                uint sourceId = al.GenSource();
                sourcePool.Enqueue(sourceId);
                allSources.Add(sourceId);
            }
        }

        public void HandleAudioCommand(AudioCommand command)
        {
            ThrowIfDisposed();

            // TODO Later perhapse break down the commands to a dictionary dispatch system and register commands to functions
            // Can help keep growth scalable and keep up response speed, a switch is fine, but still

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
                    
                    try
                    {
                        if (!sourcePool.TryDequeue(out uint newSourceID))
                        {
                            sourceHandleRequest.SetException(
                                new InvalidOperationException("No OpenAL sources available."));

                            return;
                        }

                        sourceHandleRequest.SetResult(new OpenALSourceHandle(newSourceID));
                    }
                    catch (Exception e)
                    { 
                        sourceHandleRequest.SetException(e); 
                    }

                    break;

                case RequestNewBufferHandleCommand requestNewBufferHandleCommand:

                    requestNewBufferHandleCommand.Deconstruct(out string key,out TaskCompletionSource<ISoundBufferHandle> bufferHandleRequest);

                    try
                    {
                        if (!bufferCache.TryGetValue(key, out uint cachedBuffer))
                        {
                            PCMAudioData PCMData = audioAssetProvider.LoadAudioData(key);

                            OpenALBufferHandle bufferRequestHandle = new OpenALBufferHandle(al.GenBuffer());

                            UploadBuffer(bufferRequestHandle, PCMData); // TODO This is unsafe, please deal with ASAP, testing for now

                            bufferCache.Add(key, bufferRequestHandle.Id);

                            bufferToKey.Add(bufferRequestHandle.Id, key);

                            bufferHandleRequest.SetResult(bufferRequestHandle);

                            return;
                        }

                        bufferHandleRequest.SetResult(new OpenALBufferHandle(cachedBuffer));
                    }
                    catch (Exception e) 
                    { 
                        bufferHandleRequest.SetException(e); 
                    }

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


                    BindSourceToBuffer(alPlaySourceHandle.Id, alPlayBufferHandle.Id);

                    al.SourcePlay(alPlaySourceHandle.Id);

                    break;

                case PausePlayerCommand pausePlayerCommand:

                    pausePlayerCommand.Deconstruct(out ISoundSourceHandle pauseSourceHandle);

                    if (pauseSourceHandle is not OpenALSourceHandle pauseALSourceHandle)
                        throw new InvalidCastException();

                    al.SourcePause(pauseALSourceHandle.Id);

                    break;

                case SetPlayerVolumeCommand setPlayerVolume:

                    setPlayerVolume.Deconstruct(out ISoundSourceHandle volumeSoureHandle, out float volume);

                    if (volumeSoureHandle is not OpenALSourceHandle volumeALSourceHandle)
                        throw new InvalidCastException();

                    al.SetSourceProperty(volumeALSourceHandle.Id, SourceFloat.Gain, volume);

                    break;

                case SetPlayerLoopingCommand setPlayerLoopingCommand:

                    setPlayerLoopingCommand.Deconstruct(out ISoundSourceHandle isLoopingSource, out bool isLooping);

                    if (isLoopingSource is not OpenALSourceHandle isLoopingAlSource)
                        throw new InvalidCastException();

                    al.SetSourceProperty(isLoopingAlSource.Id, SourceBoolean.Looping, isLooping);

                    break;

                case SetPlayerPitchCommand setPlayerPitchCommand:

                    setPlayerPitchCommand.Deconstruct(out ISoundSourceHandle pitchSoundSoruce, out float pitch);

                    if (pitchSoundSoruce is not OpenALSourceHandle pitchALSource)
                        throw new InvalidCastException();

                    al.SetSourceProperty(pitchALSource.Id, SourceFloat.Pitch, pitch);

                    break;

                case SetPlayerPositionCommand setPlayerPositionCommand:

                    setPlayerPositionCommand.Deconstruct(out ISoundSourceHandle positionSoundSoruce, out Vector2 position);

                    if (positionSoundSoruce is not OpenALSourceHandle positionALSoundSoruce)
                        throw new InvalidCastException();

                    al.SetSourceProperty(positionALSoundSoruce.Id, SourceVector3.Position, position.X, position.Y, 0f);

                    break;

                case ReleaseSourceCommand releaseSourceCommand:

                    releaseSourceCommand.Deconstruct(out ISoundSourceHandle soundSoruceToRelease);

                    if (soundSoruceToRelease is not OpenALSourceHandle soundALSoruceToRelease)
                        throw new InvalidCastException();

                    ReleaseSourceAndCheckBuffer(soundALSoruceToRelease.Id);

                    break;
            }
        }

        private void HandleListenerCommand(SoundListenerCommand command)
        {
            switch (command)
            {
                case SetListenerGainCommand setListenerGainCommand:

                    setListenerGainCommand.Deconstruct(out float gain);

                    al.SetListenerProperty(ListenerFloat.Gain, gain);

                    break;

                case SetListenerPositionCommand setListenerPositionCommand:

                    setListenerPositionCommand.Deconstruct(out Vector2 position);

                    al.SetListenerProperty(ListenerVector3.Position, position.X, position.Y, 0);

                    break;
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

        private void ResetSourceForReuse(uint source)
        {
            al.SourceStop(source);
            al.SetSourceProperty(source, SourceInteger.Buffer, 0);
        }

        private void DeleteSource(uint source)
        {
            al.SourceStop(source);
            al.SetSourceProperty(source, SourceInteger.Buffer, 0);
            al.DeleteSource(source);
        }

        private void ReleaseSourceAndCheckBuffer(uint source)
        {
            ResetSourceForReuse(source);

            if (!sourceToBuffer.TryGetValue(source, out uint bufferUsed))
            {
                sourcePool.Enqueue(source);
                return;
            }

            sourceToBuffer.Remove(source);

            if (bufferToSource.TryGetValue(bufferUsed, out List<uint>? sources))
            {
                sources.Remove(source);

                if (sources.Count == 0)
                {
                    bufferToSource.Remove(bufferUsed);
                    ReleaseBuffer(bufferUsed);
                }
            }

            sourcePool.Enqueue(source);
        }

        private void BindSourceToBuffer(uint source, uint buffer)
        {
            if (sourceToBuffer.TryGetValue(source, out uint oldBuffer))
            {
                if (bufferToSource.TryGetValue(oldBuffer, out List<uint>? oldSources))
                {
                    oldSources.Remove(source);

                    if (oldSources.Count == 0)
                    {
                        bufferToSource.Remove(oldBuffer);
                        ReleaseBuffer(oldBuffer);
                    }
                }
            }

            if (!bufferToSource.TryGetValue(buffer, out List<uint>? sources))
            {
                sources = new List<uint>();
                bufferToSource.Add(buffer, sources);
            }

            if (!sources.Contains(source))
            {
                sources.Add(source);
            }

            sourceToBuffer[source] = buffer;
        }

        private void ReleaseBuffer(uint buffer)
        {
            if (!bufferToKey.TryGetValue(buffer, out string? key))
                return;

            bufferCache.Remove(key);
            bufferToKey.Remove(buffer);

            al.DeleteBuffer(buffer);
        }

        public void Dispose()
        {
            foreach (uint source in allSources)
            {
                DeleteSource(source);
            }

            uint[] buffers = bufferCache.Values.ToArray();

            foreach (uint buffer in buffers)
            {
                ReleaseBuffer(buffer);
            }

            sourcePool.Clear();

            allSources.Clear();

            sourceToBuffer.Clear();

            bufferToSource.Clear();

            isDisposed = true;
        }
    }
}
