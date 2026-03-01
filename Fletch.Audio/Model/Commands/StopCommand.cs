namespace Fletch.Audio.Model.Commands
{
    internal record StopCommand(int SoundPlayerId) : AudioCommand(SoundPlayerId);
}
