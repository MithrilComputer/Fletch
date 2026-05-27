using System.Numerics;

namespace Fletch.Audio.Model.SoundPlayerCommands
{
    internal record SetPlayerPositionCommand(ISoundSourceHandle SoundPlayerHandle, Vector2 Position) : SoundPlayerCommand();
}
