using System;
using NAudio.Wave;
using SonicThinking.scripts.nodes;

namespace SonicThinking.scripts.sample_providers;

public class CachingSampleProvider : ISampleProvider
{
    public CachingSampleProvider(ISampleProvider source)
    {
        _source = source;
    }

    public void Force(int offset, int count)
    {
        var need = offset + count;
        var have = _data?.Length ?? 0;
        
        Array.Resize(ref _data, need);

        if (have < need)
        {
            // Read more samples
            var result = _source.Read(_data, offset, count);
            Array.Resize(ref _data, result + offset);
        }
    }

    public void Clear()
    {
        _data = null;
    }
    
    public int Read(float[] buffer, int offset, int count)
    {
        Force(offset, count);
        _data.CopyTo(buffer, offset);
        return _data.Length;
    }

    public WaveFormat WaveFormat => NANode.DefaultWaveFormat;

    private ISampleProvider _source;
    private float[] _data;
}