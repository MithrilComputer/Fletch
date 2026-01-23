using Fletch.Engine.Hierarchy;

namespace Fletch.Engine.Abstractions.Factories
{
    internal interface IGameObjectFactory
    {
        GameObject BuildGameObject(uint gameObjectID);
    }
}
