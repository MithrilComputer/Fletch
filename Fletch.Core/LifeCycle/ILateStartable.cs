namespace Fletch.Core.LifeCycle
{
    internal interface ILateStartable
    {
        void OnLateStart();

        bool HasLateStarted { get; set; }
    }
}
