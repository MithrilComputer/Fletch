using Fletch.Input.Abstractions.Backends;
using Fletch.Input.Abstractions.InputDevices;

namespace Fletch.Input.API
{
    public class InputService
    {
        private readonly IInputBackend backend;

        /// <summary>
        /// The keyboard device.
        /// </summary>
        public IKeyboard Keyboard { get; }

        /// <summary>
        /// All gamepad slots.
        /// </summary>
        public IReadOnlyList<IGamepad> Gamepads { get; }

        /// <summary>
        /// The mouse device.
        /// </summary>
        public IMouse Mouse { get; }

        /// <summary>
        /// The maximum number of supported controllers.
        /// </summary>
        public byte MaxControllerCount { get; }

        /// <summary>
        /// True if at least one gamepad is currently connected.
        /// </summary>
        public bool AnyGamepadConnected => backend.AnyGamepadConnected;

        /// <summary>
        /// Number of currently connected gamepads.
        /// </summary>
        public int NumberOfConnectedGamepads => backend.NumberOfConnectedGamepads;

        /// <summary>
        /// Event fired when a controller is connected.
        /// </summary>
        public event Action<int>? OnControllerConnected
        {
            add => backend.OnControllerConnected += value;
            remove => backend.OnControllerConnected -= value;
        }

        /// <summary>
        /// Event fired when a controller is disconnected.
        /// </summary>
        public event Action<int>? OnControllerDisconnected
        {
            add => backend.OnControllerDisconnected += value;
            remove => backend.OnControllerDisconnected -= value;
        }

        internal InputService(IInputBackend backend)
        {
            this.backend = backend;

            Keyboard = backend.KeyboardDevice;
            Gamepads = backend.GamepadSlots;
            Mouse = backend.MouseDevice;

            MaxControllerCount = backend.MaxControllerCount;
        }
    }
}
