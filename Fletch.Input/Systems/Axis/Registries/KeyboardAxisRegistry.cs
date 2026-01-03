using Fletch.Input.Model;

namespace Fletch.Input.Systems.Axis.Registries
{
    internal class KeyboardAxisRegistry
    {
        private readonly Dictionary<string, KeyCode> keyboardPositiveAxisRegistry;
        private readonly Dictionary<string, KeyCode> keyboardNegativeAxisRegistry;

        public bool TryGetKeycodeFromAxis(bool isPositive, string axisName, out KeyCode keyCode)
        {
            if (isPositive)
            {
                return keyboardPositiveAxisRegistry.TryGetValue(axisName, out keyCode);
            }
            else
            {
                return keyboardNegativeAxisRegistry.TryGetValue(axisName, out keyCode);
            }
        }

        public bool RegisterKeycodeToAxis(bool isPositive, string axisName, KeyCode keyCode)
        {
            if (isPositive)
            {
                if (keyboardPositiveAxisRegistry.ContainsKey(axisName))
                {
                    return false;
                }
                keyboardPositiveAxisRegistry[axisName] = keyCode;
                return true;
            }
            else
            {
                if (keyboardNegativeAxisRegistry.ContainsKey(axisName))
                {
                    return false;
                }
                keyboardNegativeAxisRegistry[axisName] = keyCode;
                return true;
            }
        }
    }
}
