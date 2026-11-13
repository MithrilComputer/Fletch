using Fletch.Engine.Hierarchy.Objects;
using Fletch.Engine.Systems.Scenes;

namespace Fletch.Engine.Abstractions.Factories
{
    internal interface ISceneObjectFactory
    {
        GameObject BuildGameObject(uint gameObjectID, Scene host);

        UIObject BuildUIObject(uint uiObjectID, Scene host);
    }
}
