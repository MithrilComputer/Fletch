using Fletch.Core.Colors;
using Fletch.Engine.Hierarchy;
using Fletch.Engine.Systems.Scenes;
using Fletch.Physics.Components;
using Fletch.Physics.Components.CollisionShapes;
using Fletch.Physics.Model;
using Fletch.Rendering.Abstractions.Factories;
using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.API;
using Fletch.Rendering.Components;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Fletch.Sandbox
{
    internal class WorldScrollingSystem
    {
        private readonly List<GameObject> scrollingObjects = new List<GameObject>();

        public float PlaySpeed { get; set; } = 1f;

        public float BarHeightVariance { get; set; } = 4f;

        public float BarStartPos { get; set; } = 3f;

        
        private float distanceSinceLastBar = 0f;

        private float barGap = 2f;


        private readonly Scene level;

        private readonly Random random;


        private readonly ITexture barTexture;

        public WorldScrollingSystem(Scene level, ITextureFactory textureFactory)
        {
            this.level = level;

            random = new Random();

            barTexture = textureFactory.CreateSolidColor(1, 1, Color.Green);
        }

        private void MoveWorld(float deltaTime)
        {
            foreach (GameObject obj in scrollingObjects)
            {
                RigidBody body = obj.GetComponent<RigidBody>();

                if(body == null)
                {
                    Vector2 position = obj.Transform.LocalPosition;

                    position.X -= PlaySpeed * deltaTime;

                    obj.Transform.LocalPosition = position;

                    return;
                }

                //body.LinearVelocity = new Vector2(-PlaySpeed, 0f);
            }
        }

        private void SpawnBar()
        {
            float height = random.NextSingle() * BarHeightVariance + 1f;

            GameObject bar = level.CreateGameObject();

            RigidBody body = bar.AddComponent<RigidBody>();

            bar.Transform.LocalScale = new Vector2(0.5f, height);

            SpriteRenderer renderer = bar.AddComponent<SpriteRenderer>();

            renderer.VisualResource.Texture = barTexture;

            renderer.VisualResource.PixelPerWorldUnit = 1;

            body.PhysicsMode = PhysicsMode.Kinematic;

            body.Position = new Vector2(BarStartPos, -3.5f + height / 2f);

            BoxCollider collider = body.AddCollider<BoxCollider>();
            collider.Width = 0.5f;
            collider.Height = height;

            body.LinearVelocity = new Vector2(-PlaySpeed, 0f);

            scrollingObjects.Add(bar);
        }

        public void UpdateWorld(float deltaTime)
        {
            MoveWorld(deltaTime);

            distanceSinceLastBar += PlaySpeed * deltaTime;

            if (distanceSinceLastBar >= barGap)
            {
                //SpawnBar();
                distanceSinceLastBar -= barGap;
            }
        }

    }
}
