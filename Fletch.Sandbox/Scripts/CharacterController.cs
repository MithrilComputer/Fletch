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

        private readonly float jumpForce = 1.3f;

        private readonly float jumpCooloffTime = 0.1f;


        private readonly float pitchRange = 0.5f;


        private readonly IGameContext context;


        public event Action? OnJump;

        public event Action? OnDeath;


        private RigidBody rb;

        private SoundPlayer jumpSoundPlayer;

        private SoundPlayer deathSoundPlayer;

        private Random random;

        private Vector2 playerStartPosition = new Vector2(-5f, 0f);


        public bool JumpPressed { get; private set; } = false;

        public bool DownDashPressed { get; private set; } = false;


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

            rb.OnTriggerEnter += OnTriggerEnter;

            rb.Position = playerStartPosition;

            AudioSource audioSource = GameObject.GetComponent<AudioSource>();

            audioSource.TryCreateSoundPlayer(
                Path.Combine(context.PathProvider.AssetFolderDirectory, "jump.wav"),
                out SoundPlayer JSoundPlayer);

            JSoundPlayer.IsSpatial = false;

            JSoundPlayer.Volume = 2f;

            jumpSoundPlayer = JSoundPlayer;

            audioSource.TryCreateSoundPlayer(
                Path.Combine(context.PathProvider.AssetFolderDirectory, "hurt.wav"),
                out SoundPlayer DSoundPlayer);

            DSoundPlayer.IsSpatial = false;

            DSoundPlayer.Volume = 2f;

            deathSoundPlayer = DSoundPlayer;
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

            if (!GameState.Ended)
            {
                MoveCharater();
            }
        }

        public override void FixedUpdate(FixedTimeStep deltaTime)
        {
            if (!GameState.Started)
                return;

            rb.AddForce(Vector2.UnitY * -fallForce);
        }

        public void ResetState()
        {
            rb.Position = playerStartPosition;
            rb.LinearVelocity = Vector2.Zero;
            rb.Rotation = 0f;

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
            else
            {
                JumpPressed = false;
            }

            if (
                input.Keyboard.GetKeyDown(KeyCode.S) ||
                input.Mouse.GetButtonDown(MouseButtonCode.Right) ||
                input.Keyboard.GetKeyDown(KeyCode.LeftControl)
                )
            {
                DownDashPressed = true;
            }
            else
            {
                DownDashPressed = false;
            }
        }

        private void MoveCharater()
        {
            if (JumpPressed && !jumpCoolingOff)
            {
                jumpSoundPlayer.Pitch = 1 + ((2 * (float)random.NextDouble()) - 1) * pitchRange;

                jumpSoundPlayer.PlaySound();

                OnJump?.Invoke();

                rb.LinearVelocity = new Vector2(0f, 0f);

                rb.AddImpulse(Vector2.UnitY * jumpForce);

                jumpCoolingOff = true;
            }

            if (DownDashPressed && !jumpCoolingOff)
            {
                jumpSoundPlayer.Pitch = 1 + ((2 * (float)random.NextDouble()) - 1) * pitchRange;

                jumpSoundPlayer.PlaySound();

                OnJump?.Invoke();

                rb.LinearVelocity = new Vector2(0f, 0f);

                rb.AddImpulse(Vector2.UnitY * -jumpForce);

                jumpCoolingOff = true;
            }
        }

        private void OnCollisionEnter(Collider self, Collider other)
        {
            if (!GameState.Running)
                return;

            deathSoundPlayer.PlaySound();

            rb.AddImpulse(new Vector2((float)random.NextDouble(), (float)random.NextDouble()) * 3f);

            OnDeath?.Invoke();
        }

        private void OnTriggerEnter(Collider trigger, Collider visitor)
        {
            if (!GameState.Running)
                return;

            deathSoundPlayer.PlaySound();
        }
    }
}