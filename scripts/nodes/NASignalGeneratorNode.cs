using System;
using Godot;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SonicThinking.scripts.autoload;
using SonicThinking.scripts.sample_providers;

namespace SonicThinking.scripts.nodes;

public partial class NASignalGeneratorNode : NANode
{
	public NASignalGeneratorNode()
	{
		_cache = new CachingSampleProvider(_generator);
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Compositor>("/root/Compositor").ForceCache += _cache.Force;
		GetNode<Compositor>("/root/Compositor").ClearCache += _cache.Clear;
		
		_frequency = GetNode<SpinBox>("Frequency/SpinBox");
		_gainSlider = GetNode<HSlider>("GainSlider");
		_typeButton = GetNode<OptionButton>("WaveForm/OptionButton");
		_invertRight = GetNode<CheckBox>("InvertRight");

		_frequency.ValueChanged += OnFrequencyChanged;
		_gainSlider.ValueChanged += OnGainChanged;
		_typeButton.ItemSelected += index => { _generator.Type = ResolveWaveType((int)index); };
		_invertRight.Toggled += pressed => _generator.PhaseReverse[0] = pressed; 
		
		_generator.Gain = _gainSlider.Value;
		_generator.Frequency = _frequency.Value;
		_generator.Type = ResolveWaveType(_typeButton.Selected);
		_generator.PhaseReverse[0] = _invertRight.ButtonPressed;

		InputChanged += OnInputChanged;
	}

	private void OnInputChanged(NANode sender, int slotIndex, ISampleProvider input)
	{
		if (slotIndex != FrequencySlot) throw new IndexOutOfRangeException("Invalid input slot.");
		_frequencyInput = input;
		_frequency.Editable = input == null;
	}

	private SignalGeneratorType ResolveWaveType(int selected)
	{
		return (SignalGeneratorType)selected;
	}

	private void OnGainChanged(double value)
	{
		_generator.Gain = value;
	}

	private void OnFrequencyChanged(double value)
	{
		_generator.Frequency = value;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (_frequencyInput != null)
		{
			float[] f = new float[1];
			_frequencyInput.Read(f, 0, 1);
			_generator.Frequency = f[0];
			_frequency.Value = f[0];
		}
	}

	protected override ISampleProvider GetOutput(int port)
	{
		return _cache;
	}

	public const int OutputSlot = 1;
	private const int FrequencySlot = 0;

	private readonly SignalGenerator _generator = new SignalGenerator(DefaultSampleRate, DefaultChannelCount);
	private readonly CachingSampleProvider _cache;
	
	private ISampleProvider _frequencyInput;
	private SpinBox _frequency;
	private HSlider _gainSlider;
	private OptionButton _typeButton;
	private CheckBox _invertRight;
}