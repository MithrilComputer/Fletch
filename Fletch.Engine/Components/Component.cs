namespace Fletch.Engine.Components
{
    public abstract class Component
    {
        public bool IsEnabled { get; private set; } = true;

        public event Action<bool>? EnableStateChanged;

        public void ChangeEnableState(bool enabled)
        {
            if (IsEnabled == enabled)
                return;

            IsEnabled = enabled;

            EnableStateChanged?.Invoke(enabled);
        }

        protected abstract void NotifyComponentChanged();

        internal virtual void OnRemoved()
        {
        }
    }
}