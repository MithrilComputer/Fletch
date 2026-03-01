using System.Numerics;

namespace Fletch.Audio.Model.Commands
{
    internal record SetPositionCommand(int SoundPlayerId, Vector2 Position) : AudioCommand(SoundPlayerId);
}
