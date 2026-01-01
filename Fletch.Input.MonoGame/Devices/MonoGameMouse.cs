using Fletch.Input.Abstractions.InputDevices;
using Fletch.Input.Model;
using Microsoft.Xna.Framework.Input;

namespace Fletch.Input.MonoGame.Devices
{
    internal class MonoGameMouse : IMouse
    {
        private MouseState currentMouseState;
        private MouseState previousMouseState;

        public CursorState CursorState { get; set; } = CursorState.Visible;

        /// <summary>
        /// The current cumulative position of the mouse wheel since the start of the game in ticks.
        /// </summary>
        public int WheelPosition => currentMouseState.ScrollWheelValue;

        /// <summary>
        /// The current X position of the mouse cursor in relation to the window.
        /// </summary>
        /// <remarks> This value is in pixels. </remarks>
        public int X => currentMouseState.X;

        /// <summary>
        /// The current Y position of the mouse cursor in relation to the window.
        /// </summary>
        /// <remarks> This value is in pixels. </remarks>
        public int Y => currentMouseState.Y;

        /// <summary>
        /// The change in the mouse wheel position since the last update.
        /// </summary>
        public int WheelDelta => currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue;

        /// <summary>
        /// Change in the X position of the mouse cursor since the last update.
        /// </summary>
        public int DeltaX => currentMouseState.X - previousMouseState.X;

        /// <summary>
        /// Change in the Y position of the mouse cursor since the last update.
        /// </summary>
        public int DeltaY => currentMouseState.Y - previousMouseState.Y;

        public MonoGameMouse(MouseState initialState)
        {
            currentMouseState = initialState;
            previousMouseState = initialState;
        }

        /// <summary>
        /// Updates the mouse state.
        /// </summary>
        public void UpdateMouseState(MouseState mouseState)
        {
            previousMouseState = currentMouseState;
            currentMouseState = mouseState;
        }

        /// <summary>
        /// Gets the current state of the specified mouse button.
        /// </summary>
        public bool GetButton(MouseButtonCode buttonCode)
        {
            return GetButtonFromCode(currentMouseState, buttonCode);
        }

        /// <summary>
        /// Gets whether the specified mouse button was pressed this frame.
        /// </summary>
        public bool GetButtonDown(MouseButtonCode buttonCode)
        {
            bool current = GetButtonFromCode(currentMouseState, buttonCode);
            bool previous = GetButtonFromCode(previousMouseState, buttonCode);

            return current && !previous;
        }

        /// <summary>
        /// Gets whether the specified mouse button was released this frame.
        /// </summary>
        public bool GetButtonUp(MouseButtonCode buttonCode)
        {
            bool current = GetButtonFromCode(currentMouseState, buttonCode);
            bool previous = GetButtonFromCode(previousMouseState, buttonCode);

            return !current && previous;
        }

        /// <summary>
        /// Gets the state of the specified mouse button from the given mouse state.
        /// </summary>
        private static bool GetButtonFromCode(MouseState state, MouseButtonCode buttonCode)
        {
            switch (buttonCode)
            {
                case MouseButtonCode.Left:
                    return state.LeftButton == ButtonState.Pressed;

                case MouseButtonCode.Right:
                    return state.RightButton == ButtonState.Pressed;

                case MouseButtonCode.Middle:
                    return state.MiddleButton == ButtonState.Pressed;

                case MouseButtonCode.Button4:
                    return state.XButton1 == ButtonState.Pressed;

                case MouseButtonCode.Button5:
                    return state.XButton2 == ButtonState.Pressed;

                default:
                    return false;
            }
        }
    }
}
