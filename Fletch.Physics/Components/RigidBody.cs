using Fletch.Engine.Components;
using Fletch.Physics.Model;
using System.Numerics;

namespace Fletch.Physics.Components
{
    // TODO At some point I should prob make a way of setting specfic stuff as dirty, I feel like making the whole thing sync is a bit wastefull.

    /// <summary>
    /// TODO
    /// </summary>
    public sealed class RigidBody : GameObjectComponent
    {
        internal bool IsDirty { get; private set; }


        private float mass = 1f;

        private float gravityScale = 1f;


        private float linearDrag;

        private float angularDrag;


        private Vector2 linearVelocity;

        private float angularVelocity;


        private bool useInterpolation;

        private bool fixedRotation;


        private PhysicsMode physicsMode = PhysicsMode.Dynamic;


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

        public void AddForce(Vector2 force) { } //TODO

        public void AddImpulse(Vector2 impulse) { } //TOD

        public void AddTorque(float torque) { } // TODO 
    }
}