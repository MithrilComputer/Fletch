using Fletch.Input.Model;

namespace Fletch.Input.Abstractions.InputDevices
{
    /// <summary>
    /// The keyboard input device abstraction.
    /// </summary>
    internal interface IKeyboard
    {
        /// <summary>
        /// Gets the current state of the specified key.
        /// </summary>
        public bool GetKey(KeyCode keyCode);

        /// <summary>
        /// Gets whether the specified key was pressed this frame.
        /// </summary>
        public bool GetKeyDown(KeyCode keyCode);

        /// <summary>
        /// Gets whether the specified key was released this frame.
        /// </summary>
        public bool GetKeyUp(KeyCode keyCode);
    }
}
