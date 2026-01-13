namespace Fletch.Engine.Components.Updateable
{
    internal interface IUpdate : IComponent
    {
        void Update(float deltaTime);

        void OnStart();
    }
}
