using Fletch.Audio.Abstractions.Backend;
using Fletch.Audio.Model;
using Fletch.Audio.Model.SoundPlayerCommands.Buffer;
using Fletch.Audio.Model.SoundPlayerCommands.Source;

namespace Fletch.Audio.Factories.SoundPlayers
{
    internal class SoundPlayerFactory
    {
        private readonly IAudioBackend audioBackend;

        public SoundPlayerFactory(IAudioBackend audioBackend) 
        {
            this.audioBackend = audioBackend;
        }

        public SoundPlayer Create(string key) 
        { 
            SoundPlayer player = new SoundPlayer();

            Task.Run(() => LoadSoundAssetsToPlayer(player, key));

            return player;
        }

        /// <summary>
        /// Loads a sound asset identified by the specified key and assigns it to the given sound player.
        /// </summary>
        /// <param name="player">The sound player to assign the loaded sound asset to.</param>
        /// <param name="soundKey">The key identifying the sound asset to load.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task LoadSoundAssetsToPlayer(SoundPlayer player, string soundKey)
        {

            //TODO marshal the assignment back to the engine to complete at a safe point.

            TaskCompletionSource<ISoundSourceHandle> sourceSource = new TaskCompletionSource<ISoundSourceHandle>(); // Soure of the Audio Source

            RequestNewSourceCommand sourceRequest = new RequestNewSourceCommand(sourceSource);

            audioBackend.SendCommand(sourceRequest);

            ISoundSourceHandle soundSourceHandle = await sourceSource.Task;

            if (soundSourceHandle != null) // TODO throw an error if null
            {
                player.AssignSoundSource(soundSourceHandle); 
            } 

            TaskCompletionSource<ISoundBufferHandle> bufferSource = new TaskCompletionSource<ISoundBufferHandle>();

            RequestNewBufferHandleCommand bufferRequest = new RequestNewBufferHandleCommand(soundKey, new TaskCompletionSource<ISoundBufferHandle>());

            audioBackend.SendCommand(bufferRequest);

            ISoundBufferHandle bufferHandle = await bufferRequest.Result.Task;

            if (bufferHandle != null) // TODO throw an error if null
            {
                player.AssignSoundBuffer(bufferHandle);
            }
        }
    }
}
