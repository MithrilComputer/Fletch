namespace Fletch.Audio.Model.SoundPlayerCommands
{
    internal record PlayPlayerCommand(ISoundSourceHandle SoundPlayerHandle, ISoundBufferHandle SoundBufferHandle) : SoundPlayerCommand();
}
