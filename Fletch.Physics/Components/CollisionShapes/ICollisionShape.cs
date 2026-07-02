using Fletch.Physics.Model;

namespace Fletch.Physics.Components.CollisionShapes
{
    internal interface ICollisionShape 
    {
        PhysicsMaterial Material { get; }
    }
}
