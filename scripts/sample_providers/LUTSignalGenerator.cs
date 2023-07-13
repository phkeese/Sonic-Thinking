using System;
using NAudio.Wave;
using SonicThinking.scripts.nodes;

namespace SonicThinking.scripts.sample_providers;

class LUTSignalGenerator : ISampleProvider
{
    private readonly double[] _waveTable;
    private double _phase;
    private readonly ISampleProvider _frequencySource;

    public LUTSignalGenerator(ISampleProvider frequencySource)
    {
        if (!Equals(frequencySource.WaveFormat, NANode.DefaultWaveFormat))
        {
            throw new ArgumentException("Frequency provider must have single channel, project sample rate and float encoding.");
        }
        
        int sampleRate = WaveFormat.SampleRate;
        
        _waveTable = new double[sampleRate];
        for (int index = 0; index < sampleRate; ++index)
            _waveTable[index] = (float)Math.Sin(2 * Math.PI * (double)index / sampleRate);
        // For sawtooth instead of sine: waveTable[index] = (float)index / sampleRate;

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
            float phaseStep = _waveTable.Length * (frequency / WaveFormat.SampleRate);
            
            int waveTableIndex = (int)_phase % _waveTable.Length;
            buffer[n + offset] = (float)(this._waveTable[waveTableIndex]);
            _phase += phaseStep;
            if (_phase > (double)_waveTable.Length)
                _phase -= (double)_waveTable.Length;
        }
        return count;
    }

    private float[] ReadFrequency(int bufferSize, int offset, int count)
    {
        var buffer = new float[bufferSize];
        _frequencySource.Read(buffer, offset, count);
        return buffer;
    }
}