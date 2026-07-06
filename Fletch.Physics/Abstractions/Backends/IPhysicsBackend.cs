using Fletch.Core.Math.Geometry;
using Fletch.Physics.Components.CollisionShapes;
using Fletch.Physics.Model;
using Fletch.Physics.Model.ResourceHandles;
using Fletch.Physics.Model.ShapeData;
using System.Numerics;

namespace Fletch.Physics.Abstractions.Backends
{
    internal interface IPhysicsBackend
    {
        IWorldHandle CreateWorld(Vector2 gravity);

        IBodyHandle CreateBody(Vector2 Position, float rotation, PhysicsMode physicsMode);

        IColliderHandle CreateCollider(IBodyHandle body, PhysicsMaterial physicsMaterial, ShapeData shapeData);

        
    }
}
