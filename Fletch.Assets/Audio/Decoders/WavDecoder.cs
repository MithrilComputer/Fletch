using DrWavDotNet;
using Fletch.Assets.Audio.Abstractions;
using Fletch.Audio.Model.AudioData;

namespace Fletch.Assets.Audio.Decoders
{
    internal class WavDecoder : IAudioDecoder // TODO This was made with a tutorial, very new to decoding so make sure to re-read later
    {
        public bool CanDecode(string path)
        {
            return Path.GetExtension(path).Equals(".wav", StringComparison.OrdinalIgnoreCase);
        }

        public PCMAudioData Decode(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            using DrWav drwav = new DrWav();

            if (!drwav.InitFile(path))
                throw new InvalidOperationException($"Failed to load WAV file: {path}");

            uint sampleRate = drwav.SampleRate;

            int channels = drwav.Channels;

            ulong totalFrameCount = drwav.TotalPCMFrameCount;

            if (channels != 1 && channels != 2)
            {
                throw new NotSupportedException(
                    $"Only mono and stereo WAV files are supported. Channels: {channels}");
            }

            ulong totalSampleCount = totalFrameCount * (ulong)channels;

            if (totalSampleCount > int.MaxValue)
            {
                throw new NotSupportedException("WAV file is too large.");
            }

            short[] samples = new short[(int)totalSampleCount];

            ulong framesRead = drwav.ReadPcmFramesS16(
                (uint)totalFrameCount,
                samples
            );

            if(framesRead != totalFrameCount)
            {
                throw new InvalidOperationException(
                    $"Failed to read full WAV file. Expected {totalFrameCount} frames, got {framesRead}.");
            }

            byte[] pcmBytes = new byte[samples.Length * sizeof(short)]; // I think?

            Buffer.BlockCopy(
                samples,
                0,
                pcmBytes,
                0,
                pcmBytes.Length
            );

            return new PCMAudioData(
                data: pcmBytes,
                sampleRate: sampleRate,
                channels: (byte)channels,
                sampleFormat: AudioSampleFormat.Signed16
            );
        }
    }
}
