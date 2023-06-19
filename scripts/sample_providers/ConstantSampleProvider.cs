using NAudio.Wave;
using SonicThinking.scripts.nodes;

namespace SonicThinking.scripts.sample_providers;

public class ConstantSampleProvider : ISampleProvider
{
    public int Read(float[] buffer, int offset, int count)
    {
        for (int i = offset; i < offset + count; ++i)
        {
            buffer[i] = Value;
        }

        return count;
    }

    public WaveFormat WaveFormat { get; } = NANode.DefaultWaveFormat;
    public float Value;
}