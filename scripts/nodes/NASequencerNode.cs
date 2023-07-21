using System;
using Godot;
using Godot.Collections;
using NAudio.Wave;
using SonicThinking.scripts.helpers;
using SonicThinking.scripts.sample_providers;

namespace SonicThinking.scripts.nodes;

public partial class NASequencerNode : NANode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timingBox = GetNode<SpinBox>("Timing/SpinBox");
		_removeButton = GetNode<Button>("Buttons/Remove");
		
		foreach (var child in GetChildren())
		{
			if (child.IsInGroup("internal"))
			{
				RemoveChild(child);
				AddChild(child, false, InternalMode.Front);
			}
		}
		AddStep();
	}

	private SliderInput AddStep()
	{
		_removeButton.Disabled = false;
		
		var FrequencyEdit = GD.Load<PackedScene>("res://scenes/helpers/frequency_edit.tscn");
		var instance = FrequencyEdit.Instantiate<SliderInput>();

		int slot_index = GetChildCount(true);
		AddChild(instance);
		
		SetSlotEnabledLeft(slot_index, true);
		SetSlotColorLeft(slot_index, NANode.SignalColor(SignalType.Frequency));
		SetSlotTypeLeft(slot_index, (int)SignalType.Frequency);

		return instance;
	}

	private void RemoveStep()
	{
		if (GetChildCount() == 0)
			return;

		var last = GetChild(GetChildCount() - 1);
		RemoveChild(last);
		last.QueueFree();

		if (GetChildCount() == 0)
		{
			_removeButton.Disabled = true;
		}
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_time += delta;
		if (_nextTrigger <= _time)
		{
			int sequenceLength = GetChildCount();
			_sequenceIndex = (_sequenceIndex + 1) % sequenceLength;
			var input = GetChild<SliderInput>(_sequenceIndex);
			_constant.Value = (float)input.Value;
			_nextTrigger = _time + _timingBox.Value;
		}
	}

	
	private readonly ConstantSampleProvider _constant = new ConstantSampleProvider();
	private SpinBox _timingBox;
	private Button _removeButton;
	
	private double _time = 0.0;
	private double _nextTrigger = 0.0;
	private int _sequenceIndex = 0;

	private const int SequenceOutput = 0;
	public override Dictionary Serialize()
	{
		return new Dictionary()
		{
			{ "timing", _timingBox.Value },
			{ "steps", GetSteps() },
		};
	}

	private double[] GetSteps()
	{
		var steps = new double[GetChildCount()];
		for (int i = 0; i < GetChildCount(); i++)
		{
			steps[i] = (float)GetChild<SliderInput>(i).Value;
		}

		return steps;
	}

	public override void Deserialize(Dictionary state)
	{
		_timingBox.Value = state["timing"].AsDouble();
		
		// Ensure there are enough steps
		var steps = state["steps"].AsFloat64Array();
		while (GetChildCount() < steps.Length)
		{
			AddStep();
		}

		for (int i = 0; i < steps.Length; i++)
		{
			GetChild<SliderInput>(i).Value = steps[i];
		}
	}

	protected override ISampleProvider GetOutput(int port)
	{
		var slot = GetConnectionOutputSlot(port);
		if (slot != SequenceOutput) throw new IndexOutOfRangeException("Invalid output slot.");
		return _constant;
	}
}