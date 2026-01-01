using Fletch.Input.Model;
using System.Numerics;

namespace Fletch.Input.Abstractions.InputDevices
{
    internal interface IGamepad
    {
        /// <summary>
        /// The logical player ID associated with this gamepad.
        /// </summary>
        int PlayerID { get; }

        /// <summary>
        /// The current connection status of the gamepad.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// The current value of the left trigger.
        /// </summary>
        float LeftTrigger { get; }

        /// <summary>
        /// The current value of the right trigger.
        /// </summary>
        float RightTrigger { get; }

        /// <summary>
        /// The change in the left trigger value since the last update.
        /// </summary>
        float LeftTriggerDelta { get; }

        /// <summary>
        /// The change in the right trigger value since the last update.
        /// </summary>
        float RightTriggerDelta { get; }

        /// <summary>
        /// The current position of the left thumbstick.
        /// </summary>
        Vector2 LeftThumbstick { get; }

        /// <summary>
        /// The current position of the right thumbstick.
        /// </summary>
        Vector2 RightThumbstick { get; }

        /// <summary>
        /// The change in the left thumbstick position since the last update.
        /// </summary>
        Vector2 LeftThumbstickDelta { get; }

        /// <summary>
        /// The change in the right thumbstick position since the last update.
        /// </summary>
        Vector2 RightThumbstickDelta { get; }

        /// <summary>
        /// Gets the current state of the specified button.
        /// </summary>
        bool GetButton(ControllerButtonCodes buttonCode);

        /// <summary>
        /// Gets whether the specified button was pressed this frame.
        /// </summary>
        /// <param name="buttonCode"></param>
        /// <returns></returns>
        bool GetButtonDown(ControllerButtonCodes buttonCode);

        /// <summary>
        /// Gets whether the specified button was released this frame.
        /// </summary>
        bool GetButtonUp(ControllerButtonCodes buttonCode);

        /// <summary>
        /// Vibrates the controller continuously until changed or stopped.
        /// </summary>
        bool Vibrate(float leftMotor, float rightMotor);

        /// <summary>
        /// Vibrates the controller for a fixed duration.
        /// </summary>
        void Vibrate(float leftMotor, float rightMotor, float durationSeconds);
    }
}
