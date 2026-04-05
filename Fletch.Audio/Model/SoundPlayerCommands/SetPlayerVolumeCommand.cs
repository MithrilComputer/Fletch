namespace Fletch.Audio.Model.SoundPlayerCommands
{
    internal record SetPlayerVolumeCommand(ISoundSourceHandle SoundPlayerHandle, float Volume) : SoundPlayerCommand(SoundPlayerHandle);
}
