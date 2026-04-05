using System.Numerics;

namespace Fletch.Audio.Model.SoundListenerCommands
{
    internal record SetListenerPositionCommand(Vector2 Position) : SoundListenerCommand();
}
