using Fletch.Audio.Model;
using Fletch.Audio.Silk.NET.OpenAL.Model;
using Silk.NET.OpenAL;

namespace Fletch.Audio.Silk.NET.OpenAL.Natives
{
    internal sealed class OpenALRuntime : IDisposable
    {
        private readonly AL al;

        private readonly Queue<uint> sourcePool = new Queue<uint>();

        private readonly Dictionary<OpenALSourceHandle, uint> sourceHandles = new Dictionary<OpenALSourceHandle, uint>();
        private readonly Dictionary<OpenALSoundBufferHandle, uint> bufferHandles = new Dictionary<OpenALSoundBufferHandle, uint>();

        private bool isDisposed = false;

        public OpenALRuntime(AL al)
        {
            this.al = al;
        }

        public void CreateSource(OpenALSourceHandle sourceHandle)
        {
            ThrowIfDisposed();

            if (sourceHandles.ContainsKey(sourceHandle))
            {
                return;
            }

            uint source = GetOrCreateSource();
            sourceHandles.Add(sourceHandle, source);
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(OpenALRuntime));
            }
        }

        public void Dispose()
        {

        }
    }
}
