using System.Numerics;

namespace Fletch.Audio.Model
{
    internal readonly struct AudioCommand
    {
        public AudioCommandType CommandType { get; }

        public SoundHandle SoundHandle { get; }

        public uint BufferId { get; }

        public Vector2 Position { get; }
        
        public bool IsLooping { get; }

        public float Value { get; }

        public AudioCommand(
            AudioCommandType commandType,
            SoundHandle soundHandle = default,
            uint bufferId = 0,
            Vector2 position = default,
            bool isLooping = false,
            float value = 0f)
        {
            CommandType = commandType;
            SoundHandle = soundHandle;
            BufferId = bufferId;
            Position = position;
            IsLooping = isLooping;
            Value = value;
        }
    }
}
