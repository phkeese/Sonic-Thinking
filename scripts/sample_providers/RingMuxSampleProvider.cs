using System;
using System.Collections.Generic;
using Godot;
using NAudio.Wave;
using SonicThinking.scripts.nodes;

namespace SonicThinking.scripts.sample_providers;

public class RingMuxSampleProvider : ISampleProvider
{
    public int Read(float[] outputBuffer, int offset, int count)
    {
        float[][] buffers;
        lock (_sources)
        {
            // Read all sources and index.
            buffers = new float[_sources.Count][];
            for (int i = 0; i < _sources.Count; i++)
            {
                buffers[i] = new float[offset + count];
                _sources[i].Read(buffers[i], offset, count);
            }
        }

        var indices = new float[offset + count];
        _index.Read(indices, offset, count);

        // Multiplex between sources.
        for (int i = 0; i < count; i++)
        {
            var index = indices[offset + i];
            var low = Mathf.PosMod(Mathf.FloorToInt(index), buffers.Length);
            var high = Mathf.PosMod(Mathf.CeilToInt(index), buffers.Length);
            // t = fract(index)
            var t = index - Mathf.Floor(index);

            // Perform equal power crossfade to preserve volume instead of linear interpolation.
            // https://dsp.stackexchange.com/a/14755 
            var leftT = Mathf.Sqrt(1 - t);
            var rightT = Mathf.Sqrt(t);

            var s0 = buffers[low][offset + i];
            var s1 = buffers[high][offset + i];
            var s = s0 * leftT + s1 * rightT;

            outputBuffer[offset + i] = (float)s;
        }

        return count;
    }

    public WaveFormat WaveFormat => NANode.DefaultWaveFormat;

    public void Add(ISampleProvider source)
    {
        if (!Equals(source.WaveFormat, WaveFormat))
        {
            throw new ArgumentException("Invalid Ring MUX Source WaveFormat.");
        }

        lock (_sources)
        {
            _sources.Add(source);
        }
    }

    public bool Remove(ISampleProvider source)
    {
        lock (_sources)
        {
            return _sources.Remove(source);
        }
    }


    private readonly ISampleProvider _index;
    private readonly List<ISampleProvider> _sources = new List<ISampleProvider>();

    public RingMuxSampleProvider(ISampleProvider index)
    {
        if (!Equals(index.WaveFormat, WaveFormat))
        {
            throw new ArgumentException("Invalid Ring MUX Index WaveFormat.");
        }

        _index = index;
    }
}