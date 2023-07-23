using System;
using Godot;
using NAudio.Wave;
using SonicThinking.scripts.nodes;

namespace SonicThinking.scripts.sample_providers;

class LUTSignalGenerator : ISampleProvider
{
    private double _phase;
    private readonly ISampleProvider _frequencySource;
    private readonly ISampleProvider _restartSource;

    public LUTSignalGenerator(ISampleProvider frequencySource, ISampleProvider restartSource)
    {
        if (!Equals(frequencySource.WaveFormat, NANode.DefaultWaveFormat) || !Equals(restartSource.WaveFormat, NANode.DefaultWaveFormat))
        {
            throw new ArgumentException(
                "Frequency provider must have single channel, project sample rate and float encoding.");
        }

        int sampleRate = WaveFormat.SampleRate;

        _frequencySource = frequencySource;
        _restartSource = restartSource;
    }

    public WaveFormat WaveFormat => NANode.DefaultWaveFormat;

    public int Read(float[] buffer, int offset, int count)
    {
        // Read next n frequency samples
        var  (vpoCount,vpos) = ReadFrom(_frequencySource, buffer.Length, offset, count);
        var (restartCount,restarts) = ReadFrom(_restartSource, buffer.Length, offset, count);
        var valid = Math.Min(vpoCount, restartCount);
        
        for (int i = 0; i < valid; ++i)
        {
            var doRestart = restarts[offset + i] > 0.5;
            if (doRestart)
            {
                _phase = 0;
            }

            if (_phase > WaveTable.Length && Oneshot)
            {
                buffer[offset + i] = 0;
                continue;
            }
            
            float vpo = vpos[i + offset];
            float frequency = NANode.VoltageToFrequency(vpo);
            float phaseStep = WaveTable.Length * (frequency / WaveFormat.SampleRate);

            int waveTableIndex = Mathf.PosMod((int)_phase, WaveTable.Length);
            buffer[i + offset] = WaveTable[waveTableIndex];
            _phase += phaseStep;
            if (_phase > WaveTable.Length && !Oneshot)
            {
                _phase -= WaveTable.Length;
            }
        }

        return count;
    }

    private (int,float[]) ReadFrom(ISampleProvider source, int bufferLength, int offset, int count)
    {
        var buffer = new float[bufferLength];
        return (source.Read(buffer, offset, count), buffer);
    }

    public float[] WaveTable;
    public bool Oneshot = false;
}