namespace Fletch.Core.LifeCycle
{
    internal interface IStartable
    {
        void OnStart();

        bool HasStarted { get; set; }
    }
}
