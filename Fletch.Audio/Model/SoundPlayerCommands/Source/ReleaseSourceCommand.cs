namespace Fletch.Audio.Model.SoundPlayerCommands.Source
{
    internal record ReleaseSourceCommand(ISoundSourceHandle SourceHandle) : SoundPlayerCommand();
}
