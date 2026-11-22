using Fletch.Audio.Abstractions.Assets;
using Fletch.Audio.Model;
using Fletch.Core.Diagnostics;
using Silk.NET.OpenAL;
using System.Threading.Channels;

namespace Fletch.Audio.Silk.NET.OpenAL.Natives
{
    /// <summary>
    /// Manages OpenAL audio on a background thread.
    /// </summary>
    internal sealed class OpenALManager : IDisposable
    {
        private bool disposed = false;

        private bool started = false;

        private readonly Thread audioThread;

        private readonly Channel<AudioCommand> audioCommands;

        private readonly IAudioAssetProvider audioAssetProvider;

        private readonly IFletchContextLogger<OpenALManager> logger;

        private readonly SemaphoreSlim commandSignal = new(0);

        /// <summary>
        /// Creates a new OpenAL manager.
        /// </summary>
        /// <param name="audioAssetProvider">Audio asset provider.</param>
        /// <param name="logger">Logger.</param>
        public OpenALManager(IAudioAssetProvider audioAssetProvider, IFletchContextLogger<OpenALManager> logger)
        {
            this.logger = logger;

            this.audioAssetProvider = audioAssetProvider;

            audioCommands = Channel.CreateUnbounded<AudioCommand>();

            audioThread = new Thread(AudioThreadMain);
            audioThread.IsBackground = true;
            audioThread.Name = "OpenAL Audio Thread";
        }

        /// <summary>
        /// Starts the audio thread.
        /// </summary>
        /// <remarks>Can only be run once</remarks>
        /// <exception cref="ObjectDisposedException">
        /// Thrown when disposed.
        /// </exception>
        public void Start()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(OpenALManager));
            }

            if (started)
                return;

            started = true;

            audioThread.Start();
        }

        private void AudioThreadMain()
        {
            try
            {
                using OpenALContextManager contextManager = new();
                using AL al = AL.GetApi();
                using OpenALRuntime runtime = new OpenALRuntime(al, audioAssetProvider);

                ChannelReader<AudioCommand> reader = audioCommands.Reader;

                while (true)
                {
                    commandSignal.Wait();

                    if (reader.TryRead(out AudioCommand? command))
                    {
                        runtime.HandleAudioCommand(command);
                    }
                    else if (reader.Completion.IsCompleted)
                    {
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                logger.LogError($"A Fatal Error Has Occurred In The Audio Thread: {exception}");
            }
        }



        /// <summary>
        /// Queues an audio command.
        /// </summary>
        /// <param name="audioCommand">Audio command to queue.</param>
        /// <exception cref="ObjectDisposedException">
        /// Thrown when disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Throws if a command is atempted to be queued before the manager has started.
        /// </exception>
        public void QueueCommand(AudioCommand audioCommand)
        {
            if (!started)
            {
                throw new InvalidOperationException("OpenAL manager has not been started.");
            }

            if (disposed)
            {
                throw new ObjectDisposedException(nameof(OpenALManager));
            }

            if (audioCommands.Writer.TryWrite(audioCommand))
            {
                commandSignal.Release();
            }
            else
            {
                logger.LogWarning($"Failed To Queue Audio Command. Type Of: {audioCommand.GetType()}");
            }
        }

        /// <summary>
        /// Stops the audio thread and releases resources.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            audioCommands.Writer.TryComplete();

            commandSignal.Release();

            if (audioThread.IsAlive)
            {
                audioThread.Join();
            }

            commandSignal.Dispose();
        }
    }
}
