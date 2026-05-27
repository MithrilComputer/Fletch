namespace Fletch.Audio.Model.SoundPlayerCommands.Buffer
{
    internal record RequestNewBufferHandleCommand(string BufferID, TaskCompletionSource<ISoundBufferHandle> Result) : SoundPlayerCommand();
}
