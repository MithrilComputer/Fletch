namespace Fletch.Audio.Model.AudioData
{
    internal sealed class PCMAudioData
    {
        public byte[] Data { get; }

        public uint SampleRate { get; }

        public byte Channels { get; }

        public AudioSampleFormat SampleFormat { get; }

        public int ByteCount => Data.Length;

        public int BitsPerSample
        {
            get
            {
                return SampleFormat switch
                {
                    AudioSampleFormat.Unsigned8 => 8,
                    AudioSampleFormat.Signed16 => 16,
                    AudioSampleFormat.Float32 => 32,
                    _ => 0
                };
            }
        }

        public PCMAudioData(byte[] data, uint sampleRate, byte channels, AudioSampleFormat sampleFormat)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (data.Length == 0)
                throw new ArgumentException("PCM data cannot be empty.", nameof(data));

            if (sampleRate <= 0)
                throw new ArgumentOutOfRangeException(nameof(sampleRate));

            if (channels != 1 && channels != 2)
                throw new NotSupportedException("Only mono and stereo PCM audio are supported.");

            if (sampleFormat == AudioSampleFormat.Unknown)
                throw new NotSupportedException("Unknown PCM sample format.");

            Data = data;
            SampleRate = sampleRate;
            Channels = channels;
            SampleFormat = sampleFormat;
        }
    }
}
