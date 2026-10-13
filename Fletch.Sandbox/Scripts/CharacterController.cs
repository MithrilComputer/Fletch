using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy;
using Fletch.Hosting.Abstractions.Contexts;
using Fletch.Input.API;
using Fletch.Input.Model;
using Fletch.Physics.Components;
using System.Diagnostics;
using System.Numerics;

namespace Fletch.Sandbox.Scripts
{
    internal class CharacterController : Script
    {
        private readonly IGameContext context;

        private readonly float moveSpeed = 5f;

        private Vector2 moveAxis = Vector2.Zero;

        private RigidBody rb;

        public CharacterController(GameObject gameObject, IGameContext context) : base(gameObject)
        {
            this.context = context;
        }

        public override void OnStart()
        {
            rb = GameObject.GetComponent<RigidBody>();
        }

        public override void Update(FrameTime deltaTime)
        {
            HandleInput();
        }

        public override void FixedUpdate(FixedTimeStep deltaTime)
        {
            MoveCharater();
        }

        private void HandleInput()
        {
            InputService input = context.Input;

            moveAxis = Vector2.Zero;

            if (input.Keyboard.GetKey(KeyCode.W) || input.Keyboard.GetKey(KeyCode.Up))
            {
                moveAxis += new Vector2(0f , 1f);
            }

            if (input.Keyboard.GetKey(KeyCode.S) || input.Keyboard.GetKey(KeyCode.Down))
            {
                moveAxis -= new Vector2(0f, 1f);
            }

            if (input.Keyboard.GetKey(KeyCode.D) || input.Keyboard.GetKey(KeyCode.Right))
            {
                moveAxis += new Vector2(1f, 0f);
            }

            if (input.Keyboard.GetKey(KeyCode.A) || input.Keyboard.GetKey(KeyCode.Left))
            {
                moveAxis -= new Vector2(1f, 0f);
            }
        }

        private void MoveCharater()
        {
            Vector2 moveVector = moveAxis * moveSpeed;

            rb.AddForce(moveVector);
        }
    }
}
