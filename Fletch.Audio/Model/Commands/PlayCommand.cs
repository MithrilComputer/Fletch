namespace Fletch.Audio.Model.Commands
{
    internal record PlayCommand(int SoundPlayerId) : AudioCommand(SoundPlayerId);
}
