using System;
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
            throw new ArgumentException("Frequency provider must have single channel, project sample rate and float encoding.");
        }
        
        int sampleRate = WaveFormat.SampleRate;
        
        _frequencySource = frequencySource;
    }

    public WaveFormat WaveFormat => NANode.DefaultWaveFormat;

    public int Read(float[] buffer, int offset, int count)
    {
        // Read next n frequency samples
        float[] frequencies = ReadFrequency(buffer.Length, offset, count);
        
        for (int n = 0; n < count; ++n)
        {
            float frequency = frequencies[n + offset];
            float phaseStep = WaveTable.Length * (frequency / WaveFormat.SampleRate);
            
            int waveTableIndex = (int)_phase % WaveTable.Length;
            buffer[n + offset] = (float)(this.WaveTable[waveTableIndex]);
            _phase += phaseStep;
            if (_phase > (double)WaveTable.Length)
                _phase -= (double)WaveTable.Length;
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