using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy;
using Fletch.Physics.Abstractions.CollisionShapes;
using Fletch.Physics.Managers;
using Fletch.Physics.Model;
using Fletch.Physics.Model.ResourceHandles;
using Fletch.Physics.Systems;
using System.Numerics;

namespace Fletch.Physics.Components
{
    // TODO At some point I should prob make a way of setting specfic stuff as dirty, I feel like making the whole thing sync is a bit wastefull, not terrible, but not amazing.

    /// <summary>
    /// A RigidBody is a component that allows a GameObject to be affected by physics. It can have colliders attached to it, and can be affected by forces and impulses. 
    /// </summary>
    public sealed class RigidBody : GameObjectComponent
    {
        /// <summary>
        /// A list of all the colliders owned by the <see cref="RigidBody"/>.
        /// </summary>
        public IReadOnlyList<Collider> Colliders => colliders;

        private readonly List<Collider> colliders = new List<Collider>();


        /// <summary>
        /// Occurs when this collider begins making contact with another collider.
        /// </summary>
        public event Action<Collider, Collider>? OnCollisionEnter;

        /// <summary>
        /// Occurs when this collider stops making contact with another collider.
        /// </summary>
        public event Action<Collider, Collider>? OnCollisionExit;

        internal bool IsDirty { get; private set; }

        internal IBodyHandle? BodyHandle { get; private set; }

        internal RigidBodyManager RigidBodyManager { get; private set; }

        internal bool Initialized { get; private set; }

        internal PhysicsPose CurrentPose { get; set; } = new PhysicsPose(Vector2.Zero, 0f);

        internal PhysicsPose PreviousPose { get; set; } = new PhysicsPose(Vector2.Zero, 0f);


        private ColliderManager colliderManager;


        private float gravityScale = 1f;


        private float linearDrag = 0.2f;

        private float angularDrag = 0.1f;


        internal Vector2 linearVelocity = Vector2.Zero;

        internal float angularVelocity = 0f;


        private bool useInterpolation = false;

        private bool fixedRotation = false;

        private bool lockX = false;

        private bool lockY = false;


        private PhysicsMode physicsMode = PhysicsMode.Dynamic;

        public RigidBody(GameObject gameObject) : base(gameObject) { }

        /// <summary>
        /// Initalizes the RigidBody's states.
        /// </summary>
        internal void Initialize(IBodyHandle bodyHandle, RigidBodyManager rigidBodyManager, ColliderManager colliderManager, PhysicsSystem physicsSystem)
        {
            BodyHandle = bodyHandle;
            RigidBodyManager = rigidBodyManager;

            IsDirty = true;
            Initialized = true;

            this.colliderManager = colliderManager;
        }

        internal void InvokeOnCollisionEnter(Collider self, Collider other)
        {
            OnCollisionEnter?.Invoke(self, other);
        }

        internal void InvokeOnCollisionExit(Collider self, Collider other)
        {
            OnCollisionExit?.Invoke(self, other);
        }

        /// <summary>
        /// Creates a new collider with the base type of <see cref="Collider"/>
        /// </summary>
        /// <typeparam name="T">The collider type to create, must inhearit from <see cref="Collider"/>.</typeparam>
        /// <returns>A new collider of the given T, Null if the rigidbody is not initialized yet.</returns>
        /// <throws><see cref="NotSupportedException"/> if an invalid shape is given.</throws>
        public T? AddCollider<T>() where T : Collider
        {
            if (!Initialized)
                return null;

            T newCollider = colliderManager.AddCollider<T>(this);

            colliders.Add(newCollider);

            return newCollider;
        }

        /// <summary>
        /// Removes the specified collider from the internal collection and unregisters it from the collider manager.
        /// </summary>
        /// <remarks>If not initialized and the collider is present in the collection, the method returns
        /// without removing it.</remarks>
        /// <param name="collider">Collider to remove.</param>
        public void DeleteCollider(Collider collider)
        {
            if (!Initialized)
                return;

            if (!colliders.Remove(collider))
                return;

            colliderManager.Remove(collider);
        }

        public float GravityScale
        {
            get => gravityScale;
            set => SetFieldAndDirty(ref gravityScale, value);
        }

        public float LinearDrag
        {
            get => linearDrag;
            set => SetFieldAndDirty(ref linearDrag, value);
        }

        public float AngularDrag
        {
            get => angularDrag;
            set => SetFieldAndDirty(ref angularDrag, value);
        }

        public Vector2 LinearVelocity
        {
            get => linearVelocity;
            set => SetFieldAndDirty(ref linearVelocity, value);
        }

        public float AngularVelocity
        {
            get => angularVelocity;
            set => SetFieldAndDirty(ref angularVelocity, value);
        }

        public bool UseInterpolation
        {
            get => useInterpolation;
            set => SetFieldAndDirty(ref useInterpolation, value);
        }

        public bool FixedRotation
        {
            get => fixedRotation;
            set => SetFieldAndDirty(ref fixedRotation, value);
        }

        public bool LockX
        {
            get => lockX;
            set => SetFieldAndDirty(ref lockX, value);
        }

        public bool LockY
        {
            get => lockY;
            set => SetFieldAndDirty(ref lockY, value);
        }

        public PhysicsMode PhysicsMode
        {
            get => physicsMode;
            set => SetFieldAndDirty(ref physicsMode, value);
        }

        internal void ClearDirty()
        {
            IsDirty = false;
        }

        private void SetFieldAndDirty<T>(ref T field, T value)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            field = value;
            IsDirty = true;
        }

        public void AddForce(Vector2 force)
        {
            if (!Initialized)
                return;

            RigidBodyManager.ApplyForceToBody(this, force);
        }

        public void AddForce(Vector2 force, Vector2 point)
        {
            if (!Initialized)
                return;

            RigidBodyManager.ApplyForceToBodyAt(this, force, point);
        }

        public void AddImpulse(Vector2 impulse)
        {
            if (!Initialized)
                return;

            RigidBodyManager.ApplyImpulseToBody(this, impulse);
        }

        public void AddImpulse(Vector2 impulse, Vector2 point)
        {
            if (!Initialized)
                return;

            RigidBodyManager.ApplyImpulseToBodyAt(this, impulse, point);
        }

        public void ApplyTorque(float torque)
        {
            if (!Initialized)
                return;

            RigidBodyManager.ApplyTorqueToBody(this, torque);
        }

        public void ApplyAngularImpulse(float angularImpulse)
        {
            if (!Initialized)
                return;

            RigidBodyManager.ApplyAngularImpulseToBody(this, angularImpulse);
        }
    }
}