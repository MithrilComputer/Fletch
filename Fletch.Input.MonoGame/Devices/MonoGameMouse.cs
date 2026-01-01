using Fletch.Input.Abstractions.InputDevices;
using Microsoft.Xna.Framework.Input;

namespace Fletch.Input.MonoGame.Devices
{
    internal class MonoGameMouse : IMouse
    {
        MouseState currentMouseState;

        public MonoGameMouse()
        {
            currentMouseState = Mouse.GetState();

            // TODO Make it GetClickDown and GetClickUp as well
        }

        


    }
}
