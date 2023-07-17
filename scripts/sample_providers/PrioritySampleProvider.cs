using System;
using NAudio.Wave;
using SonicThinking.scripts.nodes;

namespace SonicThinking.scripts.sample_providers;

public class PrioritySampleProvider : ISampleProvider
{
    public int Read(float[] buffer, int offset, int count)
    {
        var source = Source ?? _fallback;
        return source.Read(buffer, offset, count);
    }

    public WaveFormat WaveFormat => NANode.DefaultWaveFormat;

    private readonly ISampleProvider _fallback;
    public ISampleProvider Source;

    public PrioritySampleProvider(ISampleProvider fallback)
    {
        _fallback = fallback;
        if (!Equals(_fallback.WaveFormat, WaveFormat))
        {
            throw new ArgumentException("Fallback provider must have single channel, project sample rate and float encoding.");
        }
    }
}