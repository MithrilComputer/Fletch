using Fletch.Audio.Model;
using Fletch.Audio.Model.SoundListenerCommands;
using Fletch.Audio.Model.SoundPlayerCommands;
using Silk.NET.OpenAL;
using System.Threading.Channels;

namespace Fletch.Audio.Silk.NET.OpenAL.Natives
{
    internal sealed class OpenALManager : IDisposable
    {
        private readonly Thread audioThread;

        private bool running = false;
        private bool disposed = false;

        private readonly Channel<AudioCommand> audioCommands;

        public OpenALManager()
        {
            audioCommands = Channel.CreateUnbounded<AudioCommand>();

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

            ChannelReader<AudioCommand> reader = audioCommands.Reader;

            while(reader.WaitToReadAsync().AsTask().GetAwaiter().GetResult())
            {
                while (reader.TryRead(out AudioCommand? command))
                {
                    runtime.HandleAudioCommand(command);
                }
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
