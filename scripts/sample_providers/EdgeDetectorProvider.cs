using System;
using Godot;
using NAudio.Wave;
using SonicThinking.scripts.nodes;

namespace SonicThinking.scripts.sample_providers;

public class EdgeDetectorProvider : ISampleProvider
{
    public int Read(float[] buffer, int offset, int count)
    {
        var thresholds = new float[buffer.Length];
        var sampleCount = _threshold.Read(thresholds, offset, count);
        sampleCount = Math.Min(_source.Read(buffer, offset, count), sampleCount);

        for (int i = 0; i < sampleCount; i++)
        {
            buffer[offset + i] = Detect(buffer[offset + i], thresholds[offset + i]) ? 1 : 0;
        }
        
        return sampleCount;
    }
    

    public WaveFormat WaveFormat => NANode.DefaultWaveFormat;

    public enum TriggerMode
    {
        Rising,
        Falling,
        Both,
    };

    private bool Detect(float x, float threshold)
    {
        var now = Mathf.Sign(x - threshold);
        var last = Mathf.Sign(_lastValue - threshold);
        _lastValue = x;
        if (now == last) return false;
        switch (Mode)
        {
            case TriggerMode.Rising:
                return last < now;
            case TriggerMode.Falling:
                return last > now;
            case TriggerMode.Both:
                return true;
            default:
                throw new ArgumentException("Invalid trigger mode!");
        }
    }

    public TriggerMode Mode = TriggerMode.Rising;

    private float _lastValue = 0;
    private readonly ISampleProvider _source;
    private readonly ISampleProvider _threshold;

    public EdgeDetectorProvider(ISampleProvider source, ISampleProvider threshold)
    {
        if (!Equals(source.WaveFormat, WaveFormat) || !Equals(source.WaveFormat, WaveFormat))
        {
            throw new ArgumentException("Input must have single channel, project sample rate and float encoding.");
        }
        _source = source;
        _threshold = threshold;
    }
}