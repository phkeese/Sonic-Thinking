using System;
using Godot;
using Godot.Collections;
using NAudio.Wave;
using SonicThinking.scripts.helpers;
using SonicThinking.scripts.sample_providers;

namespace SonicThinking.scripts.nodes;

public partial class NAConstantNode : NANode
{
	public override Dictionary Serialize()
	{
		return new Dictionary()
		{
			{ "min", Low.Value },
			{ "max", High.Value },
			{ "value", Slider.Value },
			{"is_exp", ExpButton.ButtonPressed},
		};
	}

	public override void Deserialize(Dictionary state)
	{
		Low.Value = state["min"].AsDouble();
		High.Value = state["max"].AsDouble();
		Slider.Value = state["value"].AsDouble();
		ExpButton.ButtonPressed = state["is_exp"].AsBool();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Slider.MinValue = Low.Value;
		Slider.MaxValue = High.Value;
		Slider.ExpEdit = ExpButton.ButtonPressed;
		Slider.ValueChanged += value => _constant.Value = (float)value;
		_constant.Value = (float)Slider.Value;

		Low.ValueChanged += value => Slider.MinValue = value;
		High.ValueChanged += value => Slider.MaxValue = value;
		ExpButton.Toggled += value => Slider.ExpEdit = value;
	}
	
	protected override ISampleProvider GetOutput(int port)
	{
		return _constant;
	}

	private readonly ConstantSampleProvider _constant = new ConstantSampleProvider();
	private SpinBox Low => GetNodeOrNull<SpinBox>("Limits/Low");
	private SpinBox High => GetNodeOrNull<SpinBox>("Limits/High");
	private SliderInput Slider => GetNodeOrNull<SliderInput>("Slider");
	private Button ExpButton => GetNodeOrNull<Button>("%Exp");
}
