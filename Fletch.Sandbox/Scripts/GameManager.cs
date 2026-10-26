using Fletch.Core.Time;
using Fletch.Engine.Components;
using Fletch.Engine.Hierarchy;
using System.Collections.Generic;

namespace Fletch.Sandbox.Scripts
{
    internal class GameManager : Script
    {
        private bool Initalized = false;

        public CharacterController playerControler;

        public List<UIComponent> userInterfaceMessages;

        private WorldScrollingSystem worldScroller;

        private bool isPlayerDead = false;

        private bool readyToStart = true;


        private float minTimeBeforeReset = 2;


        private float timeKeep = 0f;


        public GameManager(GameObject gameObject) : base(gameObject)
        {



        }

        public void Initalize(CharacterController playerControler, List<UIComponent> userInterfaceMessages, WorldScrollingSystem worldScroller)
        {
            if (Initalized)
                return;

            this.playerControler = playerControler;
            this.userInterfaceMessages = userInterfaceMessages;
            this.worldScroller = worldScroller;

            playerControler.OnDeath += OnDeath;
        }

        public override void Update(FrameTime deltaTime)
        {
            if ((playerControler.DownDashPressed || playerControler.JumpPressed) && readyToStart)
            {
                GameState.Running = true;
                GameState.Started = true;
            }

            if (isPlayerDead)
            {
                timeKeep += deltaTime.Delta;

                if (timeKeep >= minTimeBeforeReset)
                {
                    timeKeep = 0f;

                    OnReset();
                }
            }
        }

        private void OnReset()
        {
            worldScroller.ResetWorld();

            GameState.CurrentScore = 0;

            GameState.Running = false;
            GameState.Ended = false;
            GameState.Started = false;

            readyToStart = true;

            playerControler.ResetState();

            isPlayerDead = false;
        }

        private void OnDeath()
        {
            isPlayerDead = true;

            readyToStart = false;

            GameState.Running = false;
            GameState.Ended = true;

            if(GameState.CurrentScore > GameState.BestScore)
            {
                GameState.BestScore = GameState.CurrentScore;
            }
        }
    }
}
