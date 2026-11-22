using Fletch.Audio.Model.Flags;
using Fletch.Core.LifeCycle;
using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy.Objects;
using System.Numerics;

namespace Fletch.Audio.Components
{
    public sealed class AudioListener : GameObjectComponent, IUpdateable
    {
        public AudioListener(GameObject gameObject) : base(gameObject) { }

        internal AudioListenerDirtyFlags DirtyFlags { get; private set; } = AudioListenerDirtyFlags.All;

        public float Gain { get => gain; set => SetFieldAndDirty(ref gain, value, AudioListenerDirtyFlags.Gain); }

        public int Priority { get; set; } = 0;

        private float gain = 1f;

        private Vector2 lastReadPosition = Vector2.Zero;

        public void Update(FrameTime deltaTime)
        {
            Vector2 positionalOffset = lastReadPosition - GameObject.Transform.WorldPosition;

            if (positionalOffset.Length() > 0.001f)
            {
                DirtyFlags |= AudioListenerDirtyFlags.Position;
                lastReadPosition = GameObject.Transform.WorldPosition;
            }
        }

        internal void ClearDirty()
        {
            DirtyFlags = AudioListenerDirtyFlags.None;
        }

        private void SetFieldAndDirty<T>(ref T field, T value, AudioListenerDirtyFlags flag)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            field = value;
            DirtyFlags |= flag;
        }
    }
}
