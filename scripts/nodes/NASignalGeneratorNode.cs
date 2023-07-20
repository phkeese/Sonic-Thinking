using System;
using Godot;
using Godot.Collections;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SonicThinking.scripts.autoload;
using SonicThinking.scripts.helpers;
using SonicThinking.scripts.sample_providers;

namespace SonicThinking.scripts.nodes;

public partial class NASignalGeneratorNode : NANode
{
    public override Dictionary Serialize()
    {
        return new Dictionary()
        {
            { "frequency", _frequencyInput.Value },
            { "volume", _volumeInput.Value },
            { "waveform", _waveTable.Wave },
        };
    }

    public override void Deserialize(Dictionary state)
    {
        _frequencyInput.Value = state["frequency"].AsDouble();
        _volumeInput.Value = state["volume"].AsDouble();
        _waveTable.Wave = state["waveform"].AsFloat32Array();
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _frequencyInput = GetNode<SpinBox>("Frequency/Edit");
        _frequencyInput.ValueChanged += frequency => _constantFrequency.Value = (float)frequency;
        _constantFrequency.Value = (float)_frequencyInput.Value;

        _volumeInput = GetNode<SliderInput>("Volume");
        _volumeInput.ValueChanged += volume => _constantVolume.Value = (float)volume / 100;

        _constantVolume.Value = (float)_volumeInput.Value / 100;

        _waveTable = GetNode<WaveTable>("WaveTable");
        _generator.WaveTable = _waveTable.Wave;
        _waveTable.ShapeChanged += () => _waveSelect.Selected = -1;
        
        _waveSelect = GetNode<OptionButton>("Waveform/Options");
        _waveSelect.ItemSelected += index => LoadPreset((int)index);

        InputChanged += OnInputChanged;

        // Connect cache to relevant signals
        Compositor.ForceCache += _output.Force;
        Compositor.ClearCache += _output.Clear;
    }

    public override void _ExitTree()
    {
        Compositor.ForceCache -= _output.Force;
        Compositor.ClearCache -= _output.Clear;
    }

    private void OnInputChanged(NANode sender, int portIndex, ISampleProvider input)
    {
        switch (portIndex)
        {
            case FrequencyPort:
                _frequency.Source = input;
                _frequencyInput.Editable = input == null;
                break;
            case VolumePort:
                _volume.Source = input;
                _volumeInput.Editable = input == null;
                break;
            default:
                throw new IndexOutOfRangeException("Invalid input slot.");
        }
    }

    private void LoadPreset(int index)
    {
        Func<double, double> f = index switch
        {
            // Sine Wave
            0 => (phi) => Mathf.Sin(MathF.PI * 2 * phi),

            // Square
            1 => phi => (phi < 0.5 ? 1 : -1),

            // Saw Tooth
            2 => (phi) => 2 * Math.Abs(phi - 0.5 - Mathf.Floor(phi - 0.5)) - 1,

            // Triangle Wave
            3 => phi => 4 * Mathf.Abs((phi - 0.75) - Mathf.Floor((phi - 0.75) + 0.5)) - 1,

            // Random Noise
            4 => phi => Mathf.Remap(GD.Randf(), 0, 1, 1, -1),

            // Invalid Index!
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
        };
        var step = 1.0 / _waveTable.Wave.Length;
        for (int i = 0; i < _waveTable.Wave.Length; i++)
        {
            _waveTable.Wave[i] = (float)f(i * step);
        }

        _waveTable.QueueRedraw();
    }

    protected override ISampleProvider GetOutput(int port)
    {
        return _output;
    }

    private const int FrequencyPort = 0;
    private const int VolumePort = 1;

    private SpinBox _frequencyInput;
    private readonly ConstantSampleProvider _constantFrequency = new ConstantSampleProvider();
    private readonly PrioritySampleProvider _frequency;


    private readonly LUTSignalGenerator _generator;
    private WaveTable _waveTable;
    private OptionButton _waveSelect;

    private SliderInput _volumeInput;
    private readonly ConstantSampleProvider _constantVolume = new ConstantSampleProvider();
    private readonly PrioritySampleProvider _volume;
    private readonly CachingSampleProvider _output;

    public NASignalGeneratorNode()
    {
        _frequency = new PrioritySampleProvider(_constantFrequency);
        _generator = new LUTSignalGenerator(_frequency);
        _volume = new PrioritySampleProvider(_constantVolume);

        var scaledOutput = new ScalingSampleProvider(_generator, _volume);
        _output = new CachingSampleProvider(scaledOutput);
    }
}