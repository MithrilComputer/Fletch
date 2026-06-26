using Fletch.Engine.Components;

namespace Fletch.Audio.Components
{
    public sealed class AudioListener : GameObjectComponent
    {
        //TODO Add a dirty bool for others to use for updating.

        public float Gain { get; set; } = 1f;

        public int Priority { get; set; } = 0;
    }
}
