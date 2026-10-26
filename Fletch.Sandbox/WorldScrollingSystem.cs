using Fletch.Engine.Hierarchy;
using Fletch.Engine.Systems.Scenes;
using Fletch.Physics.Components;
using Fletch.Physics.Components.CollisionShapes;
using Fletch.Physics.Model;
using Fletch.Platform.Abstractions.Paths;
using Fletch.Rendering.Abstractions.Factories;
using Fletch.Rendering.Abstractions.Resources;
using Fletch.Rendering.Components;
using Fletch.Rendering.Model;
using Fletch.Sandbox.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Fletch.Sandbox
{
    internal class WorldScrollingSystem
    {
        private readonly List<GameObject> scrollingObjects = new List<GameObject>();

        public float PlaySpeed { get; set; } = 9f;


        private float BarGap { get; set; } = 10f;

        private float BarGapHeight { get; set; } = 2f;


        const float barWidth = 1.125f;
        const float barHeight = 12.5f;

        const float minY = -3.5f;
        const float maxY = 5.5f;

        const float padding = 0.5f;


        const float barStartPos = 3f;

        const float barGenEnd = 14f;


        private Transform lastBarTansform;


        private float distanceSinceLastBar = 0f;

        private readonly Scene level;

        private readonly Random random;


        private readonly ITexture barTexture;

        public WorldScrollingSystem(Scene level, ITextureFactory textureFactory, IPathProvider pathProvider)
        {
            this.level = level;

            random = new Random();

            barTexture = textureFactory.Load(Path.Combine(pathProvider.AssetFolderDirectory, "Pipe.png"));

            GenerateStartingBars();
        }

        private void GenerateStartingBars()
        {
            int barCount = (int)MathF.Ceiling((barStartPos - barGenEnd) / (barWidth - BarGap));

            for (int i = 0; i < barCount; i++)
            {
                float distance = (BarGap * i) + barStartPos;

                SpawnBar(distance);
            }
        }

        private void MoveWorld(float deltaTime)
        {
            foreach (GameObject obj in scrollingObjects)
            {
                RigidBody body = obj.GetComponent<RigidBody>();

                SelfDeleteAfterTime deleter = obj.GetComponent<SelfDeleteAfterTime>();

                if (GameState.Running)
                {
                    deleter?.Start(10f);
                }

                if (body == null)
                {
                    if (!GameState.Running)
                    {
                        continue;
                    }

                    Vector2 position = obj.Transform.LocalPosition;

                    position.X -= PlaySpeed * deltaTime;

                    obj.Transform.LocalPosition = position;

                    continue;
                }

                if (!GameState.Running)
                {
                    body.LinearVelocity = new Vector2(0f, 0f);
                    continue;
                }

                body.LinearVelocity = new Vector2(-PlaySpeed, 0f);
            }
        }

        public void ResetWorld()
        {
            foreach (GameObject obj in scrollingObjects)
            {
                RigidBody body = obj.GetComponent<RigidBody>();

                if (body == null)
                    continue;

                obj.Destroy();
            }

            GenerateStartingBars();
        }


        private void SpawnBar(float posx)
        {
            float minGapY = minY + padding + BarGapHeight / 2f;
            float maxGapY = maxY - padding - BarGapHeight / 2f;

            float gapY = minGapY +
                random.NextSingle() * (maxGapY - minGapY);

            float bottomY = gapY - BarGapHeight / 2f - barHeight / 2f;
            float topY = gapY + BarGapHeight / 2f + barHeight / 2f;

            for (int i = 0; i < 2; i++)
            {
                GameObject bar = level.CreateGameObject();

                bar.AddComponent<SelfDeleteAfterTime>();

                RigidBody body = bar.AddComponent<RigidBody>();

                body.UseInterpolation = false;

                SpriteRenderer renderer = bar.AddComponent<SpriteRenderer>();

                if (i == 1)
                {
                    renderer.Effect = SpriteEffect.FlipVertically;
                    lastBarTansform = bar.Transform;
                }

                renderer.VisualResource.Texture = barTexture;
                renderer.VisualResource.PixelPerWorldUnit = 16;

                body.PhysicsMode = PhysicsMode.Kinematic;

                body.Position = new Vector2(
                    posx,
                    i == 0 ? bottomY : topY
                );

                BoxCollider collider = body.AddCollider<BoxCollider>();
                collider.Width = barWidth;
                collider.Height = barHeight;

                scrollingObjects.Add(bar);
            }
        }

        public void UpdateWorld(float deltaTime)
        {
            MoveWorld(deltaTime);

            distanceSinceLastBar = barGenEnd - lastBarTansform.WorldPosition.X;

            while (distanceSinceLastBar >= BarGap)
            {
                float error = distanceSinceLastBar - BarGap;

                SpawnBar(barGenEnd - error);

                distanceSinceLastBar -= BarGap;
            }
        }
    }
}