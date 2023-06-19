using System;
using Godot;
using NAudio.Wave;
using SonicThinking.scripts.sample_providers;

namespace SonicThinking.scripts.nodes;

public partial class NAConstantNode : NANode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_low = GetNode<SpinBox>("Limits/Low");
		_high= GetNode<SpinBox>("Limits/High");
		_value = GetNode<HSlider>("Value");

		_value.MinValue = _low.Value;
		_value.MaxValue = _high.Value;
		
		_low.ValueChanged += value => _value.MinValue = value;
		_high.ValueChanged += value => _value.MaxValue = value;
		_value.ValueChanged += value => _constant.Value = (float)value;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	protected override ISampleProvider GetOutput(int port)
	{
		var slot = GetConnectionOutputSlot(port);
		if (slot != OutputSlot) throw new IndexOutOfRangeException("Invalid output slot.");
		return _constant;
	}

	private readonly ConstantSampleProvider _constant = new ConstantSampleProvider();
	private SpinBox _low;
	private SpinBox _high;
	private HSlider _value;
	
	private const int OutputSlot = 0;
}