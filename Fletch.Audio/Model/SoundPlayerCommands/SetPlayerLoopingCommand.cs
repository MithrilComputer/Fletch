namespace Fletch.Audio.Model.SoundPlayerCommands
{
    internal record SetPlayerLoopingCommand(ISoundSourceHandle SoundPlayerHandle, bool IsLooping) : SoundPlayerCommand(SoundPlayerHandle);
}
