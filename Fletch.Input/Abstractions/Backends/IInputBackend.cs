using Fletch.Input.Abstractions.InputDevices;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Fletch.Input.MonoGame")]

namespace Fletch.Input.Abstractions.Backends
{
    internal interface IInputBackend
    {
        /// <summary>
        /// The keyboard input device.
        /// </summary>
        IKeyboard KeyboardDevice { get; }

        /// <summary>
        /// The mouse input device.
        /// </summary>
        IMouse MouseDevice { get; }

        /// <summary>
        /// All logical gamepad slots.
        /// </summary>
        IGamepad[] GamepadSlots { get; }

        /// <summary>
        /// The maximum number of supported controllers.
        /// </summary>
        byte MaxControllerCount { get; }

        /// <summary>
        /// True if at least one gamepad is currently connected.
        /// </summary>
        bool AnyGamepadConnected { get; }

        /// <summary>
        /// Number of currently connected gamepads.
        /// </summary>
        int NumberOfConnectedGamepads { get; }

        /// <summary>
        /// Event fired when a controller is connected.
        /// </summary>
        event Action<int>? OnControllerConnected;

        /// <summary>
        /// Event fired when a controller is disconnected.
        /// </summary>
        event Action<int>? OnControllerDisconnected;

        /// <summary>
        /// Polls hardware and updates all device states.
        /// Must be called once per frame by the host.
        /// </summary>
        void UpdateBackend();
    }
}
