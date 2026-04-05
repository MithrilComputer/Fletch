using Fletch.Audio.Model.SoundListenerCommands;
using Fletch.Audio.Model.SoundPlayerCommands;
using Silk.NET.OpenAL;

namespace Fletch.Audio.Silk.NET.OpenAL.Natives
{
    internal sealed class OpenALManager : IDisposable
    {
        private readonly Thread audioThread;

        private bool running = false;
        private bool disposed = false;

        private Queue<SoundPlayerCommand> soundPlayerCommands = new Queue<SoundPlayerCommand>();
        private Queue<SoundListenerCommand> soundListenerCommands = new Queue<SoundListenerCommand>();

        public OpenALManager()
        {
            audioThread = new Thread(AudioThreadMain);
            audioThread.IsBackground = true;
            audioThread.Name = "OpenAL Audio Thread";
        }

        public void Start()
        {
            if (running)
                return;

            running = true;

            audioThread.Start();
        }

        private void AudioThreadMain()
        {
            using OpenALContextManager contextManager = new OpenALContextManager();
            using AL al = AL.GetApi();
            using OpenALRuntime runtime = new OpenALRuntime(al);

            while (running)
            {
                //TODO Send command queue to the runtime for working

                Thread.Sleep(1);
            }
        }
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            running = false;

            if (audioThread.IsAlive)
            {
                audioThread.Join();
            }
        }
    }
}
