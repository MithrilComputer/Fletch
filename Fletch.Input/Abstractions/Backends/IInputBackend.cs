using Fletch.Input.Abstractions.InputDevices;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Fletch.Input.MonoGame")]

namespace Fletch.Input.Abstractions.Backends
{
    internal interface IInputBackend
    {
        void UpdateInputStates();

        IKeyboard GetKeyboard();
    }
}
