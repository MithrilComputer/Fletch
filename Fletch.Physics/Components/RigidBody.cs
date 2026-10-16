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


        internal RigidBodyDirtyFlags DirtyFlags { get; set; } = RigidBodyDirtyFlags.All;


        /// <summary>
        /// Occurs when this collider begins making contact with another collider.
        /// </summary>
        public event Action<Collider, Collider>? OnCollisionEnter;

        /// <summary>
        /// Occurs when this collider stops making contact with another collider.
        /// </summary>
        public event Action<Collider, Collider>? OnCollisionExit;

        internal bool IsDirty => DirtyFlags != RigidBodyDirtyFlags.None;

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

        public RigidBody(GameObject gameObject) : base(gameObject) 
        {
            base.EnableStateChanged += OnEnableStateChange;
        }

        private void OnEnableStateChange(bool enabled)
        {
            DirtyFlags |= RigidBodyDirtyFlags.EnabledState;
        }

        /// <summary>
        /// Initalizes the RigidBody's states.
        /// </summary>
        internal void Initialize(IBodyHandle bodyHandle, RigidBodyManager rigidBodyManager, ColliderManager colliderManager, PhysicsSystem physicsSystem)
        {
            BodyHandle = bodyHandle;
            RigidBodyManager = rigidBodyManager;

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

        public Vector2 Position
        {
            get => CurrentPose.Position;
            set
            {
                if (CurrentPose.Position == value)
                    return;

                CurrentPose.Position = value;

                DirtyFlags |= RigidBodyDirtyFlags.Pose;
            }
        }

        public float Rotation
        {
            get => CurrentPose.Rotation;
            set
            {
                if (CurrentPose.Rotation == value)
                    return;

                CurrentPose.Rotation = value;

                DirtyFlags |= RigidBodyDirtyFlags.Pose;
            }
        }

        public float GravityScale
        {
            get => gravityScale;
            set => SetFieldAndDirty(ref gravityScale, value, RigidBodyDirtyFlags.GravityScale);
        }

        public float LinearDrag
        {
            get => linearDrag;
            set => SetFieldAndDirty(ref linearDrag, value, RigidBodyDirtyFlags.LinearDrag);
        }

        public float AngularDrag
        {
            get => angularDrag;
            set => SetFieldAndDirty(ref angularDrag, value, RigidBodyDirtyFlags.AngularDrag);
        }

        public Vector2 LinearVelocity
        {
            get => linearVelocity;
            set => SetFieldAndDirty(ref linearVelocity, value, RigidBodyDirtyFlags.LinearVelocity);
        }

        public float AngularVelocity
        {
            get => angularVelocity;
            set => SetFieldAndDirty(ref angularVelocity, value, RigidBodyDirtyFlags.AngularVelocity);
        }

        public bool UseInterpolation
        {
            get => useInterpolation;
            set => SetFieldAndDirty(ref useInterpolation, value, RigidBodyDirtyFlags.Interpolation);
        }

        public bool FixedRotation
        {
            get => fixedRotation;
            set => SetFieldAndDirty(ref fixedRotation, value, RigidBodyDirtyFlags.MotionLocks);
        }

        public bool LockX
        {
            get => lockX;
            set => SetFieldAndDirty(ref lockX, value, RigidBodyDirtyFlags.MotionLocks);
        }

        public bool LockY
        {
            get => lockY;
            set => SetFieldAndDirty(ref lockY, value, RigidBodyDirtyFlags.MotionLocks);
        }

        public PhysicsMode PhysicsMode
        {
            get => physicsMode;
            set => SetFieldAndDirty(ref physicsMode, value, RigidBodyDirtyFlags.PhysicsMode);
        }

        internal void ClearDirty()
        {
            DirtyFlags = RigidBodyDirtyFlags.None;
        }

        private void SetFieldAndDirty<T>(ref T field, T value, RigidBodyDirtyFlags flag)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            field = value;
            DirtyFlags |= flag;
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