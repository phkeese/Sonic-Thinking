using System;
using Godot;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SonicThinking.scripts.nodes;

public partial class NASignalGeneratorNode : NANode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_frequency = GetNode<SpinBox>("Frequency/SpinBox");
		_gainSlider = GetNode<HSlider>("GainSlider");
		_typeButton = GetNode<OptionButton>("WaveForm/OptionButton");

		_frequency.ValueChanged += OnFrequencyChanged;
		_gainSlider.ValueChanged += OnGainChanged;
		_typeButton.ItemSelected += index => { _generator.Type = ResolveWaveType((int)index); }; 
		
		_generator.Gain = _gainSlider.Value;
		_generator.Frequency = _frequency.Value;
		_generator.Type = ResolveWaveType(_typeButton.Selected);
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
	public override void _Process(double delta)
	{
	}

	protected override ISampleProvider GetOutput(int port)
	{
		var slot = GetConnectionOutputSlot(port);
		if (slot != OutputSlot) throw new IndexOutOfRangeException("Invalid slot index.");
		return _generator;
	}

	public const int OutputSlot = 1;

	private readonly SignalGenerator _generator = new SignalGenerator(DefaultSampleRate, DefaultChannelCount);
	private SpinBox _frequency;
	private HSlider _gainSlider;
	private OptionButton _typeButton;
}