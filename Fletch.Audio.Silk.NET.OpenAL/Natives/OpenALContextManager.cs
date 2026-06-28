using Silk.NET.OpenAL;

namespace Fletch.Audio.Silk.NET.OpenAL.Natives
{
    /// <summary>
    /// Allows use of main OpenAL stuff without dealing with Unsafe things in main backend.
    /// </summary>
    internal unsafe sealed class OpenALContextManager : IDisposable
    {
        private readonly ALContext alc = ALContext.GetApi();
        private Device* device;
        private Context* context;

        private bool disposed = false;

        /// <summary>
        /// Keeps OpenAL device/context lifetime isolated from the rest of the backend.
        /// </summary>
        public OpenALContextManager()
        {
            try
            {
                device = alc.OpenDevice(null);

                if (device == null)
                {
                    throw new InvalidOperationException("Failed to open OpenAL device.");
                }

                context = alc.CreateContext(device, null);

                if (context == null)
                {
                    throw new InvalidOperationException("Failed to create OpenAL context.");
                }

                if (!alc.MakeContextCurrent(context))
                {
                    throw new InvalidOperationException("Failed to make OpenAL context current.");
                }
            }
            catch
            {
                CleanupNativeResources();
                alc.Dispose();
                throw;
            }
        }

        private void CleanupNativeResources()
        {
            alc.MakeContextCurrent(null);

            if (context != null)
            {
                alc.DestroyContext(context);
                context = null;
            }

            if (device != null)
            {
                alc.CloseDevice(device);
                device = null;
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            CleanupNativeResources();
            alc.Dispose();
        }
    }
}
