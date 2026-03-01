namespace Fletch.Audio.Model.Commands
{
    internal record SetVolumeCommand(int SoundPlayerId, float Volume) : AudioCommand(SoundPlayerId);
}
