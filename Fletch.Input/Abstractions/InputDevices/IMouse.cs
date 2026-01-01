using Fletch.Input.Model;

namespace Fletch.Input.Abstractions.InputDevices
{
    internal interface IMouse
    {
        /// <summary>
        /// The current cumulative position of the mouse wheel since the start of the game in ticks.
        /// </summary>
        int WheelPosition { get; }

        /// <summary>
        /// The current visibility state of the mouse cursor.
        /// </summary>
        CursorState CursorState { get; }

        /// <summary>
        /// The current X position of the mouse cursor in relation to the window.
        /// </summary>
        /// <remarks> This value is in pixels. </remarks>
        int X { get; }

        /// <summary>
        /// The current Y position of the mouse cursor in relation to the window.
        /// </summary>
        /// <remarks> This value is in pixels. </remarks>
        int Y { get; }

        /// <summary>
        /// The change in the mouse wheel position since the last update.
        /// </summary>

        int WheelDelta { get; }

        /// <summary>
        /// Change in the X position of the mouse cursor since the last update.
        /// </summary>
        int DeltaX { get; }

        /// <summary>
        /// Change in the Y position of the mouse cursor since the last update.
        /// </summary>
        int DeltaY { get; }

        /// <summary>
        /// Gets the current state of the specified mouse button.
        /// </summary>
        bool GetButton(MouseButtonCode buttonCode);

        /// <summary>
        /// Gets whether the specified mouse button was pressed this frame.
        /// </summary>
        bool GetButtonDown(MouseButtonCode buttonCode);


        /// <summary>
        /// Gets whether the specified mouse button was released this frame.
        /// </summary>
        bool GetButtonUp(MouseButtonCode buttonCode);
    }
}
