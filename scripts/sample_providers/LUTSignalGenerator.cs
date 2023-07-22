using System;
using Godot;
using NAudio.Wave;
using SonicThinking.scripts.nodes;

namespace SonicThinking.scripts.sample_providers;

class LUTSignalGenerator : ISampleProvider
{
    private double _phase;
    private readonly ISampleProvider _frequencySource;

    public LUTSignalGenerator(ISampleProvider frequencySource)
    {
        if (!Equals(frequencySource.WaveFormat, NANode.DefaultWaveFormat))
        {
            throw new ArgumentException(
                "Frequency provider must have single channel, project sample rate and float encoding.");
        }

        int sampleRate = WaveFormat.SampleRate;

        _frequencySource = frequencySource;
    }

    public WaveFormat WaveFormat => NANode.DefaultWaveFormat;

    public int Read(float[] buffer, int offset, int count)
    {
        // Read next n frequency samples
        float[] vpos = ReadFrequency(buffer.Length, offset, count);

        for (int i = 0; i < count; ++i)
        {
            float vpo = vpos[i + offset];
            float frequency = NANode.VoltageToFrequency(vpo);
            float phaseStep = WaveTable.Length * (frequency / WaveFormat.SampleRate);

            int waveTableIndex = Mathf.PosMod((int)_phase, WaveTable.Length);
            buffer[i + offset] = WaveTable[waveTableIndex];
            _phase += phaseStep;
            if (_phase > WaveTable.Length)
            {
                _phase -= WaveTable.Length;
            }
        }

        return count;
    }

    private float[] ReadFrequency(int bufferSize, int offset, int count)
    {
        var buffer = new float[bufferSize];
        _frequencySource.Read(buffer, offset, count);
        return buffer;
    }

    public float[] WaveTable;
}