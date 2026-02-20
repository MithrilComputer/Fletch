using Fletch.Engine.Components;

namespace Fletch.Engine.Abstractions.Factories
{
    internal interface IComponentFactory
    {
        Component CreateNewComponent<T>() where T : Component;
    }
}
