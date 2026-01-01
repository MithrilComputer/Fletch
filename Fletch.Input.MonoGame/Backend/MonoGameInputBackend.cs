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

        public MonoGameInputBackend()
        {
            keyboardDevice = new MonoGameKeyboard();
            mouseDevice = new MonoGameMouse();
        }

        public void UpdateInputStates()
        {
            keyboardDevice.UpdateKeyboardState(Keyboard.GetState());
            
        }

        public IKeyboard GetKeyboard() => keyboardDevice;
    }
}
