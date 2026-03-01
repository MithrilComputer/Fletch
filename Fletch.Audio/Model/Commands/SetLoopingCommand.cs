namespace Fletch.Audio.Model.Commands
{
    internal record SetLoopingCommand(int SoundPlayerId, bool IsLooping) : AudioCommand(SoundPlayerId);
}
