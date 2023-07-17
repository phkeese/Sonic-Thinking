using System;
using NAudio.Wave;
using SonicThinking.scripts.nodes;

namespace SonicThinking.scripts.sample_providers;

public class ScalingSampleProvider : ISampleProvider
{
    public int Read(float[] buffer, int offset, int count)
    {
        float[] volumes = new float[buffer.Length];
        count = _volume.Read(volumes, offset, count);
        count = _source.Read(buffer, offset, count);
        for (int i = 0; i < count; i++)
        {
            buffer[i] *= volumes[i];
        }
        return count;
    }

    public WaveFormat WaveFormat => NANode.DefaultWaveFormat;

    private readonly ISampleProvider _source;
    private readonly ISampleProvider _volume;

    public ScalingSampleProvider(ISampleProvider source, ISampleProvider volume)
    {
        if (!Equals(source.WaveFormat, WaveFormat))
        {
            throw new ArgumentException("Source provider must have single channel, project sample rate and float encoding.");
        }

        if (!Equals(volume.WaveFormat, WaveFormat))
        {
            throw new ArgumentException("Volume provider must have single channel, project sample rate and float encoding.");
        }
        
        _source = source;
        _volume = volume;
    }
}