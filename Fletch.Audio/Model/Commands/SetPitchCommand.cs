namespace Fletch.Audio.Model.Commands
{
    internal record SetPitchCommand(int SoundPlayerId, float Pitch) : AudioCommand(SoundPlayerId)
}
