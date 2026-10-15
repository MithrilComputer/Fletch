using Fletch.Engine.Model;
using Fletch.Physics.Abstractions.Backends;
using Fletch.Physics.Abstractions.CollisionShapes;
using Fletch.Physics.Components;
using Fletch.Physics.Components.CollisionShapes;
using Fletch.Physics.Helpers;
using Fletch.Physics.Model;
using Fletch.Physics.Model.Info.Collisions;
using Fletch.Physics.Model.Info.IO;
using Fletch.Physics.Model.Info.Masking.Collisions;
using Fletch.Physics.Model.Info.Shapes;
using Fletch.Physics.Model.ResourceHandles;
using Fletch.Physics.Systems;
using System.Numerics;

namespace Fletch.Physics.Managers
{
    internal class ColliderManager(PhysicsSystem physicsSystem)
    {
        private TrackedSet<Collider> colliders = new TrackedSet<Collider>();

        private readonly Dictionary<IColliderHandle, Collider> colliderRegistry = new Dictionary<IColliderHandle, Collider>();

        /// <summary>
        /// Creates a new collider with the base type of <see cref="Collider"/>
        /// </summary>
        /// <typeparam name="T">The collider type to create, must inhearit from <see cref="Collider"/>.</typeparam>
        /// <param name="rigidBody">The body to attach the colider to.</param>
        /// <returns>A new collider of the given T, Null if the rigidbody is not initialized yet.</returns>
        /// <throws><see cref="NotSupportedException"/> if an invalid shape is given.</throws>
        public T AddCollider<T>(RigidBody rigidBody) where T : Collider
        {
            Collider collider;
            ColliderWriteState writeState;

            if (typeof(T) == typeof(BoxCollider))
            {
                writeState = new ColliderWriteState(
                    dirtyFlags: ColliderDirtyFlags.All,
                    new PhysicsMaterial(),
                    new RectangleData(new Vector2(1, 1)),
                    new CollisionFilter(CollisionCategory.All, CollisionCategory.All),
                    Vector2.Zero,
                    1f,
                    0f,
                    false
                    );

                collider = physicsSystem.colliderFactory.CreateNewCollider(rigidBody, writeState);
            }
            else if (typeof(T) == typeof(CapsuleCollider))
            {
                writeState = new ColliderWriteState(
                    dirtyFlags: ColliderDirtyFlags.All,
                    new PhysicsMaterial(),
                    new CapsuleData(0.5f, 2f),
                    new CollisionFilter(CollisionCategory.All, CollisionCategory.All),
                    Vector2.Zero,
                    1f,
                    0f,
                    false
                    );

                collider = physicsSystem.colliderFactory.CreateNewCollider(rigidBody, writeState);
            }
            else if (typeof(T) == typeof(CircleCollider))
            {
                writeState = new ColliderWriteState(
                    dirtyFlags: ColliderDirtyFlags.All,
                    new PhysicsMaterial(),
                    new CircleData(0.5f),
                    new CollisionFilter(CollisionCategory.All, CollisionCategory.All),
                    Vector2.Zero,
                    1f,
                    0f,
                    false
                    );

                collider = physicsSystem.colliderFactory.CreateNewCollider(rigidBody, writeState);
            }
            else
            {
                throw new NotSupportedException(
                    $"Collider type {typeof(T).Name} is not supported.");
            }

            colliders.MarkToAdd(collider);

            return (T)collider;
        }

        /// <summary>
        /// Removes a colider from the Manager.
        /// </summary>
        /// <param name="collider">The Collider to remove.</param>
        public void Remove(Collider collider)
        {
            colliders.MarkToRemove(collider);
        }
        
        /// <summary>
        /// Refreshes the collider collection and applies updated state for colliders marked as dirty to the physics
        /// backend.
        /// </summary>
        /// <remarks>Invokes colliders.Refresh(); for each collider with IsDirty true, creates a write
        /// state via PhysicsHelper.CreateColliderWriteState, sets the state on physicsSystem.PhysicsBackend, and clears
        /// the collider's dirty flag.</remarks>
        public void UpdateColliders()
        {
            foreach(Collider collider in colliders.PendingAdds)
            {
                colliderRegistry.Add(collider.ColliderHandle, collider);
            }

            foreach (Collider collider in colliders.PendingRemoves)
            {
                colliderRegistry.Remove(collider.ColliderHandle);
            }

            colliders.Refresh();

            foreach (Collider collider in colliders.Items)
            {
                if (collider.IsDirty)
                {
                    physicsSystem.PhysicsBackend.SetColliderState(collider.ColliderHandle, PhysicsHelper.CreateColliderWriteState(collider));
                    collider.ClearDirty(); // TODO Not so sure bout clearing it here, works as expected for now, just feels wrong. Like milk thats expired only by a little.
                }
            }
        }


        /// <summary>
        /// Processes a backend collision event and invokes the corresponding collision callbacks on the primary
        /// collider's rigid body.
        /// </summary>
        /// <remarks>Returns immediately if either collider identifier is not registered or resolves to
        /// null. For CollisionEventType.Enter invokes InvokeOnCollisionEnter on collider A's rigid body; for
        /// CollisionEventType.Exit invokes InvokeOnCollisionExit.</remarks>
        /// <param name="backendCollision">Backend collision event containing the identifiers of the two colliders and the collision event type.</param>
        public void HandleCollisionEvent(BackendCollisionEvent backendCollision)
        {
            if (!colliderRegistry.TryGetValue(backendCollision.A, out Collider colliderA))
                return;

            if (!colliderRegistry.TryGetValue(backendCollision.B, out Collider colliderB))
                return;

            if(backendCollision.EventType == CollisionEventType.Enter)
            {
                colliderA.RigidBody.InvokeOnCollisionEnter(colliderA, colliderB);
                colliderB.RigidBody.InvokeOnCollisionEnter(colliderB, colliderA);
            }
            else if(backendCollision.EventType == CollisionEventType.Exit)
            {
                colliderA.RigidBody.InvokeOnCollisionExit(colliderA, colliderB);
                colliderB.RigidBody.InvokeOnCollisionExit(colliderB, colliderA);
            }
        }
    }
}
