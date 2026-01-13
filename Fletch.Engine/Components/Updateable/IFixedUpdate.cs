namespace Fletch.Engine.Components.Updateable
{
    internal interface IFixedUpdate : IComponent
    {
        void FixedUpdate(float deltaTime);

        void OnStart();
    }
}
