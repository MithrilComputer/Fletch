using Fletch.Input.Abstractions.Backends;
using Fletch.Input.Abstractions.Systems.Axis;

namespace Fletch.Input.Systems.Axis
{
    internal class AxisManager : IAxisManager
    {
        private readonly IInputBackend inputBackend;

        public AxisManager(IInputBackend inputBackend)
        {
            this.inputBackend = inputBackend;
        }


    }
}
