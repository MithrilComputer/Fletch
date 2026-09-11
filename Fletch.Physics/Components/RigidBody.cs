using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Components.Updateable;
using Fletch.Engine.Model;
using Fletch.Physics.Abstractions.CollisionShapes;
using Fletch.Physics.Components.CollisionShapes;
using Fletch.Physics.Factories;
using Fletch.Physics.Model;
using Fletch.Physics.Model.Info.IO;
using Fletch.Physics.Model.Info.Masking.Collisions;
using Fletch.Physics.Model.Info.Shapes;
using Fletch.Physics.Model.ResourceHandles;
using Fletch.Physics.Systems;
using System.Numerics;

namespace Fletch.Physics.Components
{
    // TODO At some point I should prob make a way of setting specfic stuff as dirty, I feel like making the whole thing sync is a bit wastefull.

    /// <summary>
    /// TODO 
    /// </summary>
    public sealed class RigidBody : GameObjectComponent, IFixedUpdateable
    {
        internal bool IsDirty { get; private set; }

        internal IBodyHandle? BodyHandle { get; private set; }

        internal RigidBodyManager RigidBodyManager { get; private set; }

        internal bool Initialized { get; private set; }

        internal PhysicsPose CurrentPose { get; set; } = new PhysicsPose(Vector2.Zero, 0f);

        internal PhysicsPose PreviousPose { get; set; } = new PhysicsPose(Vector2.Zero, 0f);

        private TrackedSet<Collider> colliders = new TrackedSet<Collider>();

        private float mass = 1f;

        private float gravityScale = 1f;


        private float linearDrag;

        private float angularDrag;


        private Vector2 linearVelocity;

        private float angularVelocity;


        private bool useInterpolation;

        private bool fixedRotation;

        private bool lockX;

        private bool lockY;

        private PhysicsMode physicsMode = PhysicsMode.Dynamic;


        private readonly ColliderFactory colliderFactory;

        internal RigidBody(ColliderFactory colliderFactory)
        {
            this.colliderFactory = colliderFactory;
        }

        internal void Initialize(IBodyHandle bodyHandle, RigidBodyManager rigidBodyManager, )
        {
            BodyHandle = bodyHandle;
            RigidBodyManager = rigidBodyManager;

            Initialized = true;
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

            Collider collider;
            ColliderWriteState writeState;

            if (typeof(T) == typeof(BoxCollider))
            {
                writeState = new ColliderWriteState(
                    new PhysicsMaterial(),
                    new RectangleData(new Vector2(1, 1)),
                    new CollisionFilter(CollisionCategory.All, CollisionCategory.All),
                    Vector2.Zero,
                    0f,
                    false
                    );

                collider = colliderFactory.CreateNewCollider(this, writeState);
            }
            else if (typeof(T) == typeof(CapsuleCollider))
            {
                writeState = new ColliderWriteState(
                    new PhysicsMaterial(),
                    new CapsuleData(0.5f, 2f),
                    new CollisionFilter(CollisionCategory.All, CollisionCategory.All),
                    Vector2.Zero,
                    0f,
                    false
                    );

                collider = colliderFactory.CreateNewCollider(this, writeState);
            }
            else if(typeof(T) == typeof(CircleCollider))
            {
                writeState = new ColliderWriteState(
                    new PhysicsMaterial(),
                    new CircleData(0.5f),
                    new CollisionFilter(CollisionCategory.All, CollisionCategory.All),
                    Vector2.Zero,
                    0f,
                    false
                    );

                collider = colliderFactory.CreateNewCollider(this, writeState);
            }
            else
            {
                throw new NotSupportedException(
                    $"Collider type {typeof(T).Name} is not supported.");
            }

            return (T)collider;
        }

        public float Mass
        {
            get => mass;
            set => SetFieldAndDirty(ref mass, value);
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

            //TODO Addforce to the body handle

        }

        public void AddImpulse(Vector2 impulse) 
        {
            if (!Initialized)
                return;

            //TODO Add Impulse to the body handle

        }

        public void AddTorque(float torque) 
        {
            if (!Initialized)
                return;

            //TODO add torque to the body handle
        }

        public void FixedUpdate(FixedTimeStep deltaTime)
        {
            throw new NotImplementedException();
        }
    }
}