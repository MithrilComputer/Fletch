namespace Fletch.Audio.Model.SoundPlayerCommands
{
    internal record PausePlayerCommand(ISoundSourceHandle SoundPlayerHandle) : SoundPlayerCommand(SoundPlayerHandle);
}
