using NAudio.Wave;
using SonicThinking.scripts.nodes;

namespace SonicThinking.scripts.sample_providers;

public class ReadNotifyProvider : ISampleProvider
{
    public ReadNotifyProvider(ISampleProvider source)
    {
        _source = source;
    }
 
    public int Read(float[] buffer, int offset, int count)
    {
        var result = _source.Read(buffer, offset, count);
        OnRead?.Invoke(buffer, offset, count, result);
        return result;
    }

    public WaveFormat WaveFormat { get; } = NANode.DefaultWaveFormat;
    
    public delegate void ReadHandler(float[] buffer, int offset, int count, int result);

    /// <summary>
    /// Triggered after calling Read() on this object.
    /// </summary>
    public event ReadHandler OnRead;

    private ISampleProvider _source;
}