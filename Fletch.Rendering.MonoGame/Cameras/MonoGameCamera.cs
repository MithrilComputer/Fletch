using Fletch.Core.EngineConfig;
using Fletch.Core.Math.Geometry;
using Fletch.Rendering.Abstractions.Cameras;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;
using SysMatrix = System.Numerics.Matrix3x2;
using SysVector2 = System.Numerics.Vector2;

namespace Fletch.Rendering.MonoGame.Cameras
{
    internal sealed class MonoGameCamera : ICamera
    {
        private readonly BoxingViewportAdapter adapter;

        // Camera state in ENGINE WORLD (Y-up, origin bottom-left)
        private SysVector2 position;
        private float zoom = 1f;

        // Optional: keep for later if you want rotation
        private float rotation = 0f;

        private readonly float zoomMax = 100.0f;
        private readonly float zoomMin = 0.0001f;

        // Optional world bounds (Y-up)
        private bool hasBounds;
        private RectangleFloat worldBounds;

        public MonoGameCamera(BoxingViewportAdapter viewportAdapter)
        {
            adapter = viewportAdapter;
        }

        private float VirtualWidth => adapter.VirtualWidth;
        private float VirtualHeight => adapter.VirtualHeight;

        public RectangleFloat GetVisibleArea()
        {
            // Invert world->virtual to map virtual corners back to world.
            // Use world->virtual (not world->actual) because "visible area" should ignore black bars.
            SysMatrix worldToVirtual = WorldToVirtualMatrix();
            SysMatrix.Invert(worldToVirtual, out var virtualToWorld);

            // Virtual coords are SpriteBatch-like: top-left (0,0), bottom-right (VW,VH)
            var topLeftV = new SysVector2(0, 0);
            var bottomRightV = new SysVector2(VirtualWidth, VirtualHeight);

            var a = SysVector2.Transform(topLeftV, virtualToWorld);
            var b = SysVector2.Transform(bottomRightV, virtualToWorld);

            float x = MathF.Min(a.X, b.X);
            float y = MathF.Min(a.Y, b.Y);
            float w = MathF.Abs(b.X - a.X);
            float h = MathF.Abs(b.Y - a.Y);

            return new RectangleFloat(x, y, w, h);
        }

        public void ResetViewport()
        {
            // If bounds are active, re-clamp because VH/VW may have changed meaningfully.
            if (hasBounds)
                ClampToBounds();
        }

        public SysVector2 GetCameraCenter()
        {
            // In this camera, "position" is the center of the view in world space.
            return position;
        }

        public SysVector2 WorldToScreen(SysVector2 worldPosition)
        {
            // Returns ACTUAL window pixel coords (includes boxing)
            var m = WorldToActualMatrix();
            return SysVector2.Transform(worldPosition, m);
        }

        public SysVector2 ScreenToWorld(SysVector2 screenPosition)
        {
            // screenPosition is in ACTUAL window pixels.
            // Convert actual->virtual (remove boxing), then virtual->world.
            SysVector2 virtualPos = ActualToVirtual(screenPosition);

            var w2v = WorldToVirtualMatrix();
            if (!SysMatrix.Invert(w2v, out var v2w))
                return default;

            return SysVector2.Transform(virtualPos, v2w);
        }

        public bool ContainsPoint(SysVector2 point)
        {
            var r = GetVisibleArea();
            return point.X >= r.X && point.X <= r.X + r.Width &&
                   point.Y >= r.Y && point.Y <= r.Y + r.Height;
        }

        public void SetWorldBounds(RectangleFloat bounds)
        {
            worldBounds = bounds;
            hasBounds = true;
            ClampToBounds();
        }

        public SysMatrix GetViewMatrix()
        {
            // This is what you pass to SpriteBatch.Begin(transformMatrix: ...)
            // It maps WORLD (Y-up) -> ACTUAL screen pixels (Y-down) including boxing.
            return WorldToActualMatrix();
        }

        public void SetPosition(SysVector2 position)
        {
            this.position = position;
            if (hasBounds)
                ClampToBounds();
        }

        public void MoveBy(SysVector2 delta)
        {
            position += delta;
            if (hasBounds)
                ClampToBounds();
        }

        public void SetZoom(float zoom)
        {
            this.zoom = Math.Clamp(zoom, zoomMin, zoomMax);
            if (hasBounds)
                ClampToBounds();
        }

        public SysVector2 GetPosition() => position;

        public float GetZoom() => zoom;

        private SysMatrix WorldToVirtualMatrix()
        {
            float worldToPixelsScale = (EngineConfig.VirtualResolution.X / EngineConfig.WorldUnitsAcrossScreen) * zoom;

            var view =
                SysMatrix.CreateTranslation(-position) *
                SysMatrix.CreateRotation(-rotation) *
                SysMatrix.CreateScale(worldToPixelsScale);

            var center = SysMatrix.CreateTranslation(VirtualWidth * 0.5f, VirtualHeight * 0.5f);

            var flipY = SysMatrix.CreateScale(1f, -1f) * SysMatrix.CreateTranslation(0f, VirtualHeight);

            return view * center * flipY;
        }

        private static SysMatrix XnaToSys2D(Microsoft.Xna.Framework.Matrix xna)
        {
            return new SysMatrix(
                xna.M11, xna.M12,
                xna.M21, xna.M22,
                xna.M41, xna.M42
            );
        }

        private SysMatrix VirtualToActualMatrix()
        {
            // Includes both scale and translation to account for black bars.
            return XnaToSys2D(adapter.GetScaleMatrix());
        }

        private SysMatrix WorldToActualMatrix()
        {
            // world -> virtual (camera) -> actual (boxing)
            return WorldToVirtualMatrix() * VirtualToActualMatrix();
        }

        private SysVector2 ActualToVirtual(SysVector2 actual)
        {
            // Convert from actual window pixels into virtual coordinates,
            // compensating for black bars and scale.
            Viewport vp = adapter.Viewport;

            float x = actual.X - vp.X;
            float y = actual.Y - vp.Y;

            float sx = VirtualWidth / vp.Width;
            float sy = VirtualHeight / vp.Height;

            return new SysVector2(x * sx, y * sy);
        }

        private void ClampToBounds()
        {
            // Keep the camera from showing outside world bounds.
            // We clamp camera CENTER based on half visible size in world units.

            var visible = GetVisibleArea();

            float halfW = visible.Width * 0.5f;
            float halfH = visible.Height * 0.5f;

            float minX = worldBounds.X + halfW;
            float maxX = worldBounds.X + worldBounds.Width - halfW;

            float minY = worldBounds.Y + halfH;
            float maxY = worldBounds.Y + worldBounds.Height - halfH;

            // If bounds smaller than view, just pin to middle of bounds
            if (minX > maxX) position.X = worldBounds.X + worldBounds.Width * 0.5f;
            else position.X = Math.Clamp(position.X, minX, maxX);

            if (minY > maxY) position.Y = worldBounds.Y + worldBounds.Height * 0.5f;
            else position.Y = Math.Clamp(position.Y, minY, maxY);
        }
    }
}
