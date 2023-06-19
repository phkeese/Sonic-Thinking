using NAudio.Wave;

namespace SonicThinking.scripts.sample_providers;

public class NotifyOnReadProvider : ISampleProvider
{
    private ISampleProvider _source;

    public int Read(float[] buffer, int offset, int count)
    {
        var result = _source.Read(buffer, offset, count);
        PostRead(buffer, offset, count);
        return result;
    }

    public WaveFormat WaveFormat { get; private set; }

    public ISampleProvider Source
    {
        get => _source;
        set => SetSource(value);
    }

    public void SetSource(ISampleProvider value)
    {
        _source = value;
        WaveFormat = value.WaveFormat;
    }
    
    public delegate void PostReadHandler(float[] buffer, int offset, int count);

    /// <summary>
    /// Triggered whenever Read() is called on this object.
    /// </summary>
    public PostReadHandler PostRead;
}