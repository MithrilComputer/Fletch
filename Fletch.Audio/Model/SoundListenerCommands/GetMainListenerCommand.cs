namespace Fletch.Audio.Model.SoundListenerCommands
{
    internal record RequestMainListenerCommand(ISoundListener SoundListener) : SoundListenerCommand();
}
