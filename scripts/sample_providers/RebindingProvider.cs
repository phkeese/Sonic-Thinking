using System;
using NAudio.Wave;
using SonicThinking.scripts.nodes;

namespace SonicThinking.scripts.sample_providers;

/// <summary>
/// Allows rebinding a source at runtime while playing.
/// </summary>
public class RebindingProvider : ISampleProvider
{
    private ISampleProvider _source;
    private object _sourceLock = new object();


    public int Read(float[] buffer, int offset, int count)
    {
        lock (_sourceLock)
        {
            if (Source == null)
            {
                // Fill buffer with zeros
                for (int i = offset; i < offset + count; i++)
                {
                    buffer[i] = 0.0f;
                }

                return count;
            }
            else
            {
                return Source.Read(buffer, offset, count);
            }
        }
    }

    public WaveFormat WaveFormat { get; } = NANode.DefaultWaveFormat;

    public ISampleProvider Source
    {
        get => _source;
        set => SetSource(value);
    }

    public void SetSource(ISampleProvider value)
    {
        lock(_sourceLock)
        {
            _source = value;
        }
    }
}
