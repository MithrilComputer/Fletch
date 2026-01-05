using Fletch.Rendering.Abstractions.Cameras;
using Fletch.Rendering.Abstractions.Drawing;
using Fletch.Rendering.Model;
using System.Numerics;

namespace Fletch.Rendering.MonoGame.Drawing
{
    internal class MonoGameDebugRenderer
    {
        private readonly ISpriteBatcher spriteBatch;
        private readonly ICamera camera;

        public bool IsEnabled { get; set; } = true;

        public MonoGameDebugRenderer(ISpriteBatcher spriteBatch, ICamera camera)
        {
            this.spriteBatch = spriteBatch;
            this.camera = camera;
        }

        public void DrawLine(Vector2 start, Vector2 end, float thickness, Color color)
        {

        }
    }
}
