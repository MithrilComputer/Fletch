using SDL2;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.InteropServices;

namespace Fletch.Platform.MonoGame.Loader
{
    internal static class IconLoader
    {
        public static nint GetIcon(string path)
        {
            using Image<Rgba32> image = Image.Load<Rgba32>(path);

            byte[] pixels = new byte[image.Width * image.Height * 4];
            image.CopyPixelDataTo(pixels);

            GCHandle handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            try
            {
                IntPtr surface = SDL.SDL_CreateRGBSurfaceFrom(
                    handle.AddrOfPinnedObject(),
                    image.Width,
                    image.Height,
                    32,
                    image.Width * 4,
                    0x000000FF, // R
                    0x0000FF00, // G
                    0x00FF0000, // B
                    unchecked(0xFF000000) // A
                );

                if (surface == IntPtr.Zero)
                    return nint.Zero;

                return surface;
            }
            finally
            {
                handle.Free();
            }
        }

    }
}
