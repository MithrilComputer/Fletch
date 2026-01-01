using Fletch.Input.Abstractions.InputDevices;
using Fletch.Input.Model;
using Microsoft.Xna.Framework.Input;

namespace Fletch.Input.MonoGame.Devices
{
    internal class MonoGameKeyboard : IKeyboard
    {
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

        public MonoGameKeyboard(KeyboardState initState)
        {
            currentKeyboardState = initState;
            previousKeyboardState = initState;
        }

        /// <summary>
        /// Updates the keyboard state.
        /// </summary>
        public void UpdateKeyboardState(KeyboardState state)
        {
            previousKeyboardState = currentKeyboardState;

            currentKeyboardState = state;
        }

        /// <summary>
        /// Gets the current state of the specified key.
        /// </summary>
        public bool GetKeyState(KeyCode keyCode)
        {
            return currentKeyboardState.IsKeyDown(ConvertKeycodeToKeys(keyCode));
        }

        /// <summary>
        /// Gets whether the specified key was pressed this frame.
        /// </summary>
        public bool GetKeyDown(KeyCode keyCode)
        {
            Keys key = ConvertKeycodeToKeys(keyCode);

            return currentKeyboardState.IsKeyDown(key)
                && previousKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Gets whether the specified key was released this frame.
        /// </summary>
        public bool GetKeyUp(KeyCode keyCode)
        {
            Keys key = ConvertKeycodeToKeys(keyCode);

            return currentKeyboardState.IsKeyUp(key)
                && previousKeyboardState.IsKeyDown(key);
        }

        private static Keys ConvertKeycodeToKeys(KeyCode keyCode)
        {
            return (Keys)keyCode;
        }
    }
}
