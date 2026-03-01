namespace Fletch.Audio.Model.Commands
{
    internal record PauseCommand(int SoundPlayerId) : AudioCommand(SoundPlayerId);
}
