using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fletch.Sandbox
{
    internal static class PreformanceMonitor
    {
        private const int MaxFrameSamples = 16384;
        private static readonly double[] frameTimes = new double[MaxFrameSamples];
        private static double totalFrameTime;

        private static long previousAllocatedBytes;
        private static long previousGen0;
        private static long previousGen1;
        private static long previousGen2;

        private const double HitchThresholdMs = 5.0;
        private const int MaxHitchSamples = 128;

        private static int lastFrameGen0;
        private static int lastFrameGen1;
        private static int lastFrameGen2;

        private static int frames = 0;

        private readonly struct HitchSample
        {
            public readonly DateTime Timestamp;
            public readonly double FrameTimeMs;
            public readonly int Gen0;
            public readonly int Gen1;
            public readonly int Gen2;

            public HitchSample(
                DateTime timestamp,
                double frameTimeMs,
                int gen0,
                int gen1,
                int gen2)
            {
                Timestamp = timestamp;
                FrameTimeMs = frameTimeMs;
                Gen0 = gen0;
                Gen1 = gen1;
                Gen2 = gen2;
            }
        }

        private static readonly List<HitchSample> hitchLog = new List<HitchSample>(MaxHitchSamples);

        private static TimeSpan previousCpuTime;
        private static bool profilerInitialized;

        private static double lowestFrameTime = double.MaxValue;

        private static double highestFrameTime = 0d;

        private static readonly Process process = Process.GetCurrentProcess();

        private static readonly Stopwatch frameTimer = Stopwatch.StartNew();


        public static void RunFrame()
        {
            double frameTimeMs = frameTimer.Elapsed.TotalMilliseconds;
            frameTimer.Restart();

            if (!profilerInitialized)
            {
                previousAllocatedBytes = GC.GetAllocatedBytesForCurrentThread();

                previousGen0 = GC.CollectionCount(0);
                previousGen1 = GC.CollectionCount(1);
                previousGen2 = GC.CollectionCount(2);

                previousCpuTime = process.TotalProcessorTime;

                profilerInitialized = true;
                return;
            }

            if (frames < MaxFrameSamples)
                frameTimes[frames] = frameTimeMs;

            frames++;
            totalFrameTime += frameTimeMs;

            lowestFrameTime = Math.Min(lowestFrameTime, frameTimeMs);
            highestFrameTime = Math.Max(highestFrameTime, frameTimeMs);

            if (frameTimeMs >= HitchThresholdMs)
            {
                int gen0 = GC.CollectionCount(0);
                int gen1 = GC.CollectionCount(1);
                int gen2 = GC.CollectionCount(2);

                hitchLog.Add(new HitchSample(
                    DateTime.Now,
                    frameTimeMs,
                    gen0 - lastFrameGen0,
                    gen1 - lastFrameGen1,
                    gen2 - lastFrameGen2
                ));
            }

            lastFrameGen0 = GC.CollectionCount(0);
            lastFrameGen1 = GC.CollectionCount(1);
            lastFrameGen2 = GC.CollectionCount(2);

            if (totalFrameTime < 1000.0)
                return;

            double elapsedSeconds = totalFrameTime / 1000.0;

            double fps = frames / elapsedSeconds;
            double averageFrameTime = totalFrameTime / frames;

            int sampleCount = Math.Min(frames, MaxFrameSamples);

            Array.Sort(frameTimes, 0, sampleCount);

            double p95 = GetPercentile(frameTimes, sampleCount, 0.95);
            double p99 = GetPercentile(frameTimes, sampleCount, 0.99);
            double p999 = GetPercentile(frameTimes, sampleCount, 0.999);

            double onePercentLow = p99 > 0 ? 1000.0 / p99 : 0;
            double pointOnePercentLow = p999 > 0 ? 1000.0 / p999 : 0;

            long allocatedBytes = GC.GetAllocatedBytesForCurrentThread();
            long allocatedDelta = allocatedBytes - previousAllocatedBytes;

            double allocatedPerSecond = allocatedDelta / elapsedSeconds;
            double allocatedPerFrame = (double)allocatedDelta / frames;

            long gen0Count = GC.CollectionCount(0);
            long gen1Count = GC.CollectionCount(1);
            long gen2Count = GC.CollectionCount(2);

            long gen0Collections = gen0Count - previousGen0;
            long gen1Collections = gen1Count - previousGen1;
            long gen2Collections = gen2Count - previousGen2;

            long managedMemory = GC.GetTotalMemory(false);

            process.Refresh();
            long workingSet = process.WorkingSet64;
            long privateMemory = process.PrivateMemorySize64;

            TimeSpan currentCpuTime = process.TotalProcessorTime;
            double cpuSeconds = (currentCpuTime - previousCpuTime).TotalSeconds;

            double cpuUsage =
                cpuSeconds / (elapsedSeconds * Environment.ProcessorCount) * 100.0;

            Debug.WriteLine(
                $"\n- - - - - - - - Preformance Metrics Start - - - - - - \n" +
                $"FPS:                  {fps:F0} fps\n" +
                $"Average Frame Time:   {averageFrameTime:F4} ms\n" +
                $"Minimum Frame Time:   {lowestFrameTime:F4} ms\n" +
                $"Maximum Frame Time:   {highestFrameTime:F4} ms\n" +
                $"95th Percentile:      {p95:F4} ms\n" +
                $"99th Percentile:      {p99:F4} ms\n" +
                $"1% Low FPS:           {onePercentLow:F0} fps\n" +
                $"0.1% Low FPS:         {pointOnePercentLow:F0} fps\n" +
                $"----------------------------------------\n" +
                $"Allocated / Frame:    {allocatedPerFrame:F0} B\n" +
                $"Allocated / Second:   {allocatedPerSecond / 1048576.0:F3} MiB\n" +
                $"Managed Memory:       {managedMemory / 1048576.0:F2} MiB\n" +
                $"Working Set:          {workingSet / 1048576.0:F2} MiB\n" +
                $"Private Memory:       {privateMemory / 1048576.0:F2} MiB\n" +
                $"----------------------------------------\n" +
                $"Gen 0 Collections:    {gen0Collections}\n" +
                $"Gen 1 Collections:    {gen1Collections}\n" +
                $"Gen 2 Collections:    {gen2Collections}\n" +
                $"CPU Usage:            {cpuUsage:F2}%\n" +
                $"- - - - - - - - End - - - - - - - - - - - -"
            );

            foreach (HitchSample hitch in hitchLog)
            {
                Debug.WriteLine(
                    $"[HITCH {hitch.Timestamp:HH:mm:ss.fff}] \n" +
                    $"{hitch.FrameTimeMs:F3} ms | \n" +
                    $"GC:\n" +
                    $"Gen0 +{hitch.Gen0},\n" +
                    $"Gen1 +{hitch.Gen1},\n" +
                    $"Gen2 +{hitch.Gen2}\n"
                );
            }

            hitchLog.Clear();

            previousAllocatedBytes = GC.GetAllocatedBytesForCurrentThread();

            previousGen0 = GC.CollectionCount(0);
            previousGen1 = GC.CollectionCount(1);
            previousGen2 = GC.CollectionCount(2);

            previousCpuTime = process.TotalProcessorTime;

            frames = 0;
            totalFrameTime = 0;
            lowestFrameTime = double.MaxValue;
            highestFrameTime = 0;

            frameTimer.Restart();
        }

        private static double GetPercentile(
        double[] sortedSamples,
        int count,
        double percentile)
        {
            if (count == 0)
                return 0;

            double index = (count - 1) * percentile;

            int lower = (int)Math.Floor(index);
            int upper = (int)Math.Ceiling(index);

            double fraction = index - lower;

            return sortedSamples[lower] +
                (sortedSamples[upper] - sortedSamples[lower]) * fraction;
        }
    }
}
