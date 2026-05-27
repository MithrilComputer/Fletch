namespace Fletch.Audio.Model.SoundPlayerCommands
{
    internal record SetPlayerPitchCommand(ISoundSourceHandle SoundPlayerHandle, float Pitch) : SoundPlayerCommand();
}
