namespace Fletch.Audio.Model.SoundPlayerCommands
{
    internal record StopPlayerCommand(ISoundSourceHandle SoundPlayerHandle) : SoundPlayerCommand(SoundPlayerHandle);
}
