using Fletch.Audio.Model;
using Fletch.Core.EngineConfig;
using Silk.NET.OpenAL;
using System.Collections.Concurrent;

namespace Fletch.Audio.Silk.NET.OpenAL.Natives
{
    internal class OpenALManager : IAsyncDisposable
    {
        private readonly Thread worker;

        private bool isRunning = true;

        private ALContext context;

        private AL al;

        private readonly Queue<uint> sourcePool = new Queue<uint>();

        private readonly ConcurrentQueue<AudioCommand> commandQueue = new ConcurrentQueue<AudioCommand>();

        public OpenALManager()
        {
            worker = new Thread(AudioWorker);

            worker.IsBackground = true;
            worker.Start();
        }

        public SoundHandle? QueueCommand(AudioCommand command)
        {
            commandQueue.Enqueue(command);
        }

        private void AudioWorker()
        {
            OpenALNative openAL = new OpenALNative(); // Creates and sets the audio device and AL context

            context = ALContext.GetApi();

            al = AL.GetApi();

            for(int i = 0; i < EngineConfig.AudioSourcePoolSize; i++)
            {
                sourcePool.Enqueue(al.GenSource());
            }

            while (isRunning)
            {
                

                // Create and process a queue of different types, like pending adds, removes, Plays and so on.
                // Might need to create the buffers on this thread. See if that true
                // When you make a buffer, you get back an uint ID that is used to identify what buffer youd like to play
            }

            openAL.Shutdown();
        }

        public async ValueTask DisposeAsync()
        {
            isRunning = false;

            while(worker.IsAlive)
            {
                await Task.Delay(10);
            }
        }
    }
}
