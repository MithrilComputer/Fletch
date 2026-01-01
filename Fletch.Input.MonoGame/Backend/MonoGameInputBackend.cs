using Fletch.Input.Abstractions.Backends;
using Fletch.Input.Abstractions.InputDevices;
using Fletch.Input.MonoGame.Devices;
using Microsoft.Xna.Framework.Input;

namespace Fletch.Input.MonoGame.Backend
{
    internal class MonoGameInputBackend : IInputBackend
    {
        private readonly MonoGameKeyboard keyboardDevice;
        private readonly MonoGameMouse mouseDevice;
        private readonly MonoGameGamepad[] gamepadDevices;

        /// <summary>
        /// The keyboard input device.
        /// </summary>
        public IKeyboard KeyboardDevice => keyboardDevice;

        /// <summary>
        /// The mouse input device.
        /// </summary>
        public IMouse MouseDevice => mouseDevice;

        /// <summary>
        /// All Gamepad slots.
        /// </summary>
        public IGamepad[] GamepadSlots => gamepadDevices;

        /// <summary>
        /// The maximum number of supported controllers.
        /// </summary>
        public byte MaxControllerCount { get; private set; }

        /// <summary>
        /// If any gamepad is currently connected.
        /// </summary>
        public bool AnyGamepadConnected { get; private set; }

        /// <summary>
        /// Number of currently connected gamepads.
        /// </summary>
        public int NumberOfConnectedGamepads { get; private set; }

        /// <summary>
        /// Handler for when a controller is disconnected.
        /// </summary>
        public delegate void ControllerDisconnectedHandler(int PlayerID);

        /// <summary>
        /// Handler for when a controller is connected.
        /// </summary>
        public delegate void ControllerConnectedHandler(int PlayerID);

        /// <summary>
        /// Fired when a controller is disconnected.
        /// </summary>
        public event ControllerDisconnectedHandler? OnControllerDisconnected;

        /// <summary>
        /// Fired when a controller is connected.
        /// </summary>
        public event ControllerConnectedHandler? OnControllerConnected;

        public MonoGameInputBackend(byte maxControllerCount = 4)
        {
            keyboardDevice = new MonoGameKeyboard(Keyboard.GetState());
            mouseDevice = new MonoGameMouse(Mouse.GetState());

            MaxControllerCount = Math.Clamp(maxControllerCount, (byte)0, (byte)4);

            gamepadDevices = new MonoGameGamepad[maxControllerCount];

            for (int gamepadSlot = 0; gamepadSlot < gamepadDevices.Length; gamepadSlot++)
            {
                GamePadState gamepadInitialState = GamePad.GetState(gamepadSlot);

                gamepadDevices[gamepadSlot] = new MonoGameGamepad(gamepadInitialState, gamepadSlot);

                if(gamepadInitialState.IsConnected)
                {
                    OnControllerConnected?.Invoke(gamepadSlot);
                }
            }
        }

        /// <summary>
        /// Updates the states of all input devices, fires connection events, and otherwise syncs the backend with the current input state.
        /// </summary>
        public void UpdateBackend()
        {
            keyboardDevice.UpdateKeyboardState(Keyboard.GetState());
            mouseDevice.UpdateMouseState(Mouse.GetState());

            int connectedGamepadsCount = 0;

            AnyGamepadConnected = false;

            for (int gamepadSlot = 0; gamepadSlot < gamepadDevices.Length; gamepadSlot++)
            {
                GamePadState newState = GamePad.GetState(gamepadSlot);

                if (newState.IsConnected && !gamepadDevices[gamepadSlot].IsConnected)
                {
                    OnControllerConnected?.Invoke(gamepadSlot);
                }
                else if (!newState.IsConnected && gamepadDevices[gamepadSlot].IsConnected)
                {
                    OnControllerDisconnected?.Invoke(gamepadSlot);
                }

                gamepadDevices[gamepadSlot].UpdateGamepadState(newState);

                if (newState.IsConnected)
                {
                    AnyGamepadConnected = true;
                    connectedGamepadsCount++;
                }
            }

            NumberOfConnectedGamepads = connectedGamepadsCount;
        }
    }
}
