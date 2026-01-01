using Fletch.Input.Abstractions.InputDevices;
using Fletch.Input.Model;
using Microsoft.Xna.Framework.Input;
using System.Numerics;
using XnaPlayerIndex = Microsoft.Xna.Framework.PlayerIndex;

namespace Fletch.Input.MonoGame.Devices
{
    internal class MonoGameGamepad : IGamepad
    {
        private GamePadState currentControllerState;
        private GamePadState previousControllerState;

        private DateTime vibrationEndTime = DateTime.MinValue;

        /// <summary>
        /// For compatibility with MonoGame backend, the player index associated with this gamepad.
        /// </summary>
        public XnaPlayerIndex MonoGamePlayerIndex { get; init; }

        /// <summary>
        /// The player ID associated with this gamepad.
        /// </summary>
        public int PlayerID { get; init; }

        /// <summary>
        /// The current connection status of the gamepad.
        /// </summary>
        public bool IsConnected => currentControllerState.IsConnected;

        /// <summary>
        /// The current value of the left trigger.
        /// </summary>
        public float LeftTrigger => currentControllerState.Triggers.Left;

        /// <summary>
        /// The current value of the right trigger.
        /// </summary>
        public float RightTrigger => currentControllerState.Triggers.Right;

        /// <summary>
        /// The change in the left trigger value since the last update.
        /// </summary>
        public float LeftTriggerDelta => currentControllerState.Triggers.Left - previousControllerState.Triggers.Left;

        /// <summary>
        /// The change in the right trigger value since the last update.
        /// </summary>
        public float RightTriggerDelta => currentControllerState.Triggers.Right - previousControllerState.Triggers.Right;

        /// <summary>
        /// The current position of the left thumbstick.
        /// </summary>
        public Vector2 LeftThumbstick => new(
            currentControllerState.ThumbSticks.Left.X,
            currentControllerState.ThumbSticks.Left.Y);

        /// <summary>
        /// The current position of the right thumbstick.
        /// </summary>
        public Vector2 RightThumbstick => new(
            currentControllerState.ThumbSticks.Right.X,
            currentControllerState.ThumbSticks.Right.Y);

        /// <summary>
        /// The change in the left thumbstick position since the last update.
        /// </summary>
        public Vector2 LeftThumbstickDelta => new(
            currentControllerState.ThumbSticks.Left.X - previousControllerState.ThumbSticks.Left.X,
            currentControllerState.ThumbSticks.Left.Y - previousControllerState.ThumbSticks.Left.Y);

        /// <summary>
        /// The change in the right thumbstick position since the last update.
        /// </summary>
        public Vector2 RightThumbstickDelta => new(
            currentControllerState.ThumbSticks.Right.X - previousControllerState.ThumbSticks.Right.X,
            currentControllerState.ThumbSticks.Right.Y - previousControllerState.ThumbSticks.Right.Y);

        public MonoGameGamepad(GamePadState initialState, int playerIndex )
        {
            currentControllerState = initialState;
            previousControllerState = initialState;

            PlayerID = playerIndex;

            MonoGamePlayerIndex = playerIndex switch
            {
                0 => XnaPlayerIndex.One,
                1 => XnaPlayerIndex.Two,
                2 => XnaPlayerIndex.Three,
                3 => XnaPlayerIndex.Four,
                _ => XnaPlayerIndex.One
            };
        }

        /// <summary>
        /// Updates the gamepad state.
        /// </summary>
        public void UpdateGamepadState(GamePadState gamePadState)
        {
            previousControllerState = currentControllerState;

            currentControllerState = gamePadState;

            // Check if we need to stop vibration
            if (DateTime.UtcNow >= vibrationEndTime && vibrationEndTime != DateTime.MinValue)
            {
                GamePad.SetVibration(MonoGamePlayerIndex, 0f, 0f);
                vibrationEndTime = DateTime.MinValue;
            }
        }

        /// <summary>
        /// Gets the current state of the specified controller button.
        /// </summary>
        public bool GetButton(ControllerButtonCodes buttonCode)
        {
            return GetButtonFromStateAndCode(currentControllerState, buttonCode);
        }

        /// <summary>
        /// Gets whether the specified controller button was pressed this frame.
        /// </summary>
        public bool GetButtonDown(ControllerButtonCodes buttonCode)
        {
            return GetButtonFromStateAndCode(currentControllerState, buttonCode)
                && !GetButtonFromStateAndCode(previousControllerState, buttonCode);
        }

        /// <summary>
        /// Gets whether the specified controller button was released this frame.
        /// </summary>
        public bool GetButtonUp(ControllerButtonCodes buttonCode)
        {
            return !GetButtonFromStateAndCode(currentControllerState, buttonCode)
                && GetButtonFromStateAndCode(previousControllerState, buttonCode);
        }

        /// <summary>
        /// Vibrates the controller with the specified motor speeds.
        /// </summary>
        /// <remarks>The motor speeds are clamped between 0.0 and 1.0.</remarks>
        public bool Vibrate(float leftMotor, float rightMotor)
        {
            float clampedLeftMotor = Math.Clamp(leftMotor, 0f, 1f);
            float clampedRightMotor = Math.Clamp(rightMotor, 0f, 1f);

            return GamePad.SetVibration(MonoGamePlayerIndex, clampedLeftMotor, clampedRightMotor);
        }

        /// <summary>
        /// Vibrates the controller with the specified motor speeds for a duration.
        /// </summary>
        public void Vibrate(float leftMotor, float rightMotor, float durationSeconds)
        {
            float clampedLeftMotor = Math.Clamp(leftMotor, 0f, 1f);
            float clampedRightMotor = Math.Clamp(rightMotor, 0f, 1f);

            GamePad.SetVibration(MonoGamePlayerIndex, clampedLeftMotor, clampedRightMotor);

            // Set the end time for the vibration
            vibrationEndTime = DateTime.UtcNow.AddSeconds(durationSeconds);
        }

        private static bool GetButtonFromStateAndCode(GamePadState gamepadState, ControllerButtonCodes buttonCode)
        {
            return buttonCode switch
            {
                ControllerButtonCodes.A => gamepadState.Buttons.A == ButtonState.Pressed,
                ControllerButtonCodes.B => gamepadState.Buttons.B == ButtonState.Pressed,
                ControllerButtonCodes.X => gamepadState.Buttons.X == ButtonState.Pressed,
                ControllerButtonCodes.Y => gamepadState.Buttons.Y == ButtonState.Pressed,
                ControllerButtonCodes.LeftBumper => gamepadState.Buttons.LeftShoulder == ButtonState.Pressed,
                ControllerButtonCodes.RightBumper => gamepadState.Buttons.RightShoulder == ButtonState.Pressed,
                ControllerButtonCodes.Back => gamepadState.Buttons.Back == ButtonState.Pressed,
                ControllerButtonCodes.Start => gamepadState.Buttons.Start == ButtonState.Pressed,
                ControllerButtonCodes.LeftStick => gamepadState.Buttons.LeftStick == ButtonState.Pressed,
                ControllerButtonCodes.RightStick => gamepadState.Buttons.RightStick == ButtonState.Pressed,
                ControllerButtonCodes.DPadUp => gamepadState.DPad.Up == ButtonState.Pressed,
                ControllerButtonCodes.DPadDown => gamepadState.DPad.Down == ButtonState.Pressed,
                ControllerButtonCodes.DPadLeft => gamepadState.DPad.Left == ButtonState.Pressed,
                ControllerButtonCodes.DPadRight => gamepadState.DPad.Right == ButtonState.Pressed,
                _ => false,
            };
        }
    }
}
