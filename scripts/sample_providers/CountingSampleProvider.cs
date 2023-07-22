using System;
using NAudio.Wave;
using SonicThinking.scripts.nodes;

namespace SonicThinking.scripts.sample_providers;

public class CountingSampleProvider : ISampleProvider
{
    public int Read(float[] buffer, int offset, int sampleCount)
    {
        sampleCount = _trigger.Read(buffer, offset, sampleCount);
        for (int i = 0; i < sampleCount; i++)
        {
            if (buffer[offset + i] > 0.5)
            {
                Count++;
            }

            buffer[offset + i] = Count;
        }

        return sampleCount;
    }

    /// <summary>
    /// Signal for count increment. Gives current count.
    /// </summary>
    public delegate void ChangeHandler(int count);

    /// <summary>
    /// Dispatched when the count is incremented by one.
    /// </summary>
    public ChangeHandler Changed;

    public WaveFormat WaveFormat => NANode.DefaultWaveFormat;

    private int _count = 0;

    public int Count
    {
        get => _count;
        set
        {
            _count = value;
            Changed?.Invoke(_count);
        }
    }
    private readonly ISampleProvider _trigger;

    public CountingSampleProvider(ISampleProvider trigger)
    {
        if (!Equals(trigger.WaveFormat, WaveFormat))
        {
            throw new ArgumentException("Trigger input has invalid WaveFormat.");
        }
        _trigger = trigger;
    }
}