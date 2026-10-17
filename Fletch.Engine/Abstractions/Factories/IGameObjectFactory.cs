using Fletch.Engine.Hierarchy;
using Fletch.Engine.Systems.Scenes;

namespace Fletch.Engine.Abstractions.Factories
{
    internal interface IGameObjectFactory
    {
        GameObject BuildGameObject(uint gameObjectID, Scene host);
    }
}
