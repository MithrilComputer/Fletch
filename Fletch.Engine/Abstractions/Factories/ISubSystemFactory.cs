using Fletch.Engine.Systems;

namespace Fletch.Engine.Abstractions.Factories
{
    internal interface ISubSystemFactory
    {
        T CreateSubSystem<T>() where T : SceneSubsystem;
    }
}
