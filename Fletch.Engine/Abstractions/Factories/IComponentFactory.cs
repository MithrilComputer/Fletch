using Fletch.Engine.Components;

namespace Fletch.Engine.Abstractions.Factories
{
    internal interface IComponentFactory
    {
        GameObjectComponent CreateNewComponent<T>() where T : GameObjectComponent;
    }
}
