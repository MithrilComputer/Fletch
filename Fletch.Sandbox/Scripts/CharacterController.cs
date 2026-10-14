using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy;
using Fletch.Hosting.Abstractions.Contexts;
using Fletch.Input.API;
using Fletch.Input.Model;
using Fletch.Physics.Components;
using System.Numerics;

namespace Fletch.Sandbox.Scripts
{
    internal class CharacterController : Script
    {
        private readonly float fallForce = 3f;

        private readonly float jumpForce = 1.2f;

        private readonly float jumpCooloffTime = 0.1f;


        private readonly IGameContext context;

        private RigidBody rb;


        private bool jumpPressed = false;

        private bool jumpCoolingOff = false;

        private float timeKeep = 0f;

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

            if(jumpPressed && !GameState.Started)
            {
                GameState.Started = true;
            }

            if(jumpCoolingOff)
            {
                timeKeep += deltaTime.Delta;

                if(timeKeep >= jumpCooloffTime)
                {
                    jumpCoolingOff = false;
                    timeKeep = 0f;
                }
            }
        }

        public override void FixedUpdate(FixedTimeStep deltaTime)
        {
            if (!GameState.Started)
                return;

            rb.AddForce(Vector2.UnitY * -fallForce);

            MoveCharater();
        }

        private void HandleInput()
        {
            InputService input = context.Input;

            if (
                input.Keyboard.GetKeyDown(KeyCode.Space) ||
                input.Mouse.GetButtonDown(MouseButtonCode.Left) ||
                input.Keyboard.GetKeyDown(KeyCode.W)
                )
            {
                jumpPressed = true;
            }
        }

        private void MoveCharater()
        {
            if(jumpPressed && !jumpCoolingOff)
            {
                rb.LinearVelocity = new Vector2(0f, 0f);

                rb.AddImpulse(Vector2.UnitY * jumpForce);
                jumpCoolingOff = true;
            }

            jumpPressed = false;
        }
    }
}
