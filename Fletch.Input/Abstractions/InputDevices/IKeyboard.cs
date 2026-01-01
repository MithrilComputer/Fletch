using Fletch.Input.Model;

namespace Fletch.Input.Abstractions.InputDevices
{
    internal interface IKeyboard
    {
        public bool GetKeyState(KeyCode keyCode);

        public bool GetKeyDown(KeyCode keyCode);

        public bool GetKeyUp(KeyCode keyCode);
    }
}
