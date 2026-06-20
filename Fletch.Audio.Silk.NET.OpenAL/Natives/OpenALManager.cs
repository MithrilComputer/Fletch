using Fletch.Audio.Abstractions.Assets;
using Fletch.Audio.Model;
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

        private readonly IAudioAssetProvider audioAssetProvider;

        public OpenALManager(IAudioAssetProvider audioAssetProvider)
        {
            this.audioAssetProvider = audioAssetProvider;

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
            using OpenALRuntime runtime = new OpenALRuntime(al, audioAssetProvider); 

            ChannelReader<AudioCommand> reader = audioCommands.Reader;

            while(reader.WaitToReadAsync().AsTask().GetAwaiter().GetResult())
            {
                while (reader.TryRead(out AudioCommand? command))
                {
                    runtime.HandleAudioCommand(command);
                }
            }
        }

        public void QueueCommand(AudioCommand audioCommand)
        {
            audioCommands.Writer.TryWrite(audioCommand); //TODO add some error detection and logging
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
