namespace Fletch.Audio.Model.SoundPlayerCommands
{
    internal record SetIsPlayerSpatialCommand(ISoundSourceHandle SoundPlayerHandle, bool IsSpatial) : SoundPlayerCommand();
}
