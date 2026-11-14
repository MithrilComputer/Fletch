using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy.Objects;

namespace Fletch.Engine.Abstractions.Factories
{
    internal interface IComponentFactory
    {
        GameObjectComponent CreateNewGameObjectComponent<T>(GameObject gameObject) where T : GameObjectComponent;

        UIComponent CreateNewUIComponent<T>(UIObject uiObject) where T : UIComponent;
    }
}
