using System;
using Godot;
using NAudio.Wave;
using SonicThinking.scripts.helpers;
using SonicThinking.scripts.sample_providers;

namespace SonicThinking.scripts.nodes;

public partial class NAConstantNode : NANode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Slider.MinValue = Low.Value;
		Slider.MaxValue = High.Value;
		Slider.ValueChanged += value =>
		{
			_constant.Value = (float)value;
		};

		Low.ValueChanged += value => Slider.MinValue = value;
		High.ValueChanged += value => Slider.MaxValue = value;
	}
	
	protected override ISampleProvider GetOutput(int port)
	{
		return _constant;
	}

	private readonly ConstantSampleProvider _constant = new ConstantSampleProvider();
	private SpinBox Low => GetNodeOrNull<SpinBox>("Limits/Low");
	private SpinBox High => GetNodeOrNull<SpinBox>("Limits/High");
	private SliderInput Slider => GetNodeOrNull<SliderInput>("Slider");
	
	private const int OutputSlot = 0;
}