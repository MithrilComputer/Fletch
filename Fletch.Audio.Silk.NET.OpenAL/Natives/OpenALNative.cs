using Silk.NET.OpenAL;

namespace Fletch.Audio.Silk.NET.OpenAL.Natives
{
    /// <summary>
    /// Allows use of main OpenAL stuff without dealing with Unsafe things in main backend.
    /// </summary>
    internal sealed class OpenALNative
    {
        private readonly ALContext alc = ALContext.GetApi();
        private unsafe Device* device;
        private unsafe Context* context;

        public OpenALNative()
        {
            unsafe
            {
                device = alc.OpenDevice(null);
                if (device == null)
                    throw new Exception("Failed to open OpenAL device.");

                context = alc.CreateContext(device, null);
                if (context == null)
                    throw new Exception("Failed to create OpenAL context.");

                if (!alc.MakeContextCurrent(context))
                    throw new Exception("Failed to make OpenAL context current.");
            }
        }

        public void Shutdown()
        {
            unsafe
            {
                alc.MakeContextCurrent(null);

                if (context != null) alc.DestroyContext(context);
                if (device != null) alc.CloseDevice(device);

                context = null;
                device = null;
            }
        }
    }
}
