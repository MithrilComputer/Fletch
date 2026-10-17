using Fletch.Audio.Components;
using Fletch.Audio.Model;
using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy;
using Fletch.Hosting.Abstractions.Contexts;
using Fletch.Input.API;
using Fletch.Input.Model;
using Fletch.Physics.Abstractions.CollisionShapes;
using Fletch.Physics.Components;
using System;
using System.IO;
using System.Numerics;

namespace Fletch.Sandbox.Scripts
{
    internal class CharacterController : Script
    {
        private readonly float fallForce = 3f;

        private readonly float jumpForce = 1.2f;

        private readonly float jumpCooloffTime = 0.1f;


        private readonly float pitchRange = 0.5f;


        private readonly IGameContext context;


        public event Action? OnJump;

        public event Action? OnDeath;


        private RigidBody rb;

        private SoundPlayer jumpSoundPlayer;

        private Random random;

        private Vector2 playerStartPosition = new Vector2(-5f, 0f);


        public bool JumpPressed { get; private set; } = false;


        private bool jumpCoolingOff = false;

        private float timeKeep = 0f;


        public CharacterController(GameObject gameObject, IGameContext context) : base(gameObject)
        {
            this.context = context;

            random = new Random();
        }

        public override void OnStart()
        {
            rb = GameObject.GetComponent<RigidBody>();

            rb.OnCollisionEnter += OnCollisionEnter;

            rb.Position = playerStartPosition;

            GameObject.GetComponent<AudioSource>()
                .TryCreateSoundPlayer(
                Path.Combine(context.PathProvider.AssetFolderDirectory, "jump.wav"),
                out SoundPlayer soundPlayer);

            soundPlayer.IsSpatial = false;

            soundPlayer.Volume = 0.2f;

            jumpSoundPlayer = soundPlayer;
        }

        public override void Update(FrameTime deltaTime)
        {
            HandleInput();

            if (jumpCoolingOff)
            {
                timeKeep += deltaTime.Delta;

                if (timeKeep >= jumpCooloffTime)
                {
                    jumpCoolingOff = false;
                    timeKeep = 0f;
                }
            }
        }

        public override void FixedUpdate(FixedTimeStep deltaTime)
        {
            if (!GameState.Running)
                return;

            rb.AddForce(Vector2.UnitY * -fallForce);

            MoveCharater();
        }

        public void ResetState()
        {
            rb.Position = playerStartPosition;

            jumpCoolingOff = false;
            timeKeep = 0f;
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
                JumpPressed = true;
            }
        }

        private void MoveCharater()
        {
            if (JumpPressed && !jumpCoolingOff && GameState.Running)
            {
                jumpSoundPlayer.Pitch = 1 + ((2 * (float)random.NextDouble()) - 1) * pitchRange;

                jumpSoundPlayer.PlaySound();

                OnJump?.Invoke();

                rb.LinearVelocity = new Vector2(0f, 0f);

                rb.AddImpulse(Vector2.UnitY * jumpForce);

                jumpCoolingOff = true;
            }

            JumpPressed = false;
        }

        private void OnCollisionEnter(Collider self, Collider other)
        {
            OnDeath?.Invoke();
        }
    }
}