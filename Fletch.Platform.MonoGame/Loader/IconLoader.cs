using SDL2;
using StbImageSharp;
using System.Runtime.InteropServices;

namespace Fletch.Platform.MonoGame.Loader
{
    internal static class IconLoader
    {
        // TODO AI Fix, replace later
        public static nint GetIcon(string path)
        {
            using FileStream stream = File.OpenRead(path);

            ImageResult image = ImageResult.FromStream(
                stream,
                ColorComponents.RedGreenBlueAlpha);

            nint surface = SDL.SDL_CreateRGBSurface(
                0,
                image.Width,
                image.Height,
                32,
                0x000000FF,
                0x0000FF00,
                0x00FF0000,
                unchecked(0xFF000000)
            );

            if (surface == nint.Zero)
                return nint.Zero;

            if (SDL.SDL_LockSurface(surface) != 0)
            {
                SDL.SDL_FreeSurface(surface);
                return nint.Zero;
            }

            try
            {
                SDL.SDL_Surface surfaceData =
                    Marshal.PtrToStructure<SDL.SDL_Surface>(surface);

                int rowBytes = image.Width * 4;

                for (int y = 0; y < image.Height; y++)
                {
                    Marshal.Copy(
                        image.Data,
                        y * rowBytes,
                        IntPtr.Add(surfaceData.pixels, y * surfaceData.pitch),
                        rowBytes
                    );
                }
            }
            finally
            {
                SDL.SDL_UnlockSurface(surface);
            }

            return surface;
        }
    }
}