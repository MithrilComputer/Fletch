using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy;

namespace Fletch.Engine.Abstractions.Factories
{
    internal interface IComponentFactory
    {
        GameObjectComponent CreateNewComponent<T>(GameObject gameObject) where T : GameObjectComponent;
    }
}
