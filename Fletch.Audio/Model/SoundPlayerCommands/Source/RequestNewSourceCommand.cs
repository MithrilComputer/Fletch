namespace Fletch.Audio.Model.SoundPlayerCommands.Source
{
    internal record RequestNewSourceCommand(TaskCompletionSource<ISoundSourceHandle> Result) : SoundPlayerCommand();
}
