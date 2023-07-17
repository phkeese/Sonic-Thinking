using System;
using Godot;
using NAudio.Wave;
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

	private void AddStep()
	{
		_removeButton.Disabled = false;
		
		var FrequencyEdit = GD.Load<PackedScene>("res://scenes/helpers/frequency_edit.tscn");
		var instance = FrequencyEdit.Instantiate<SpinBox>();

		int slot_index = GetChildCount(true);
		AddChild(instance);
		
		SetSlotEnabledLeft(slot_index, true);
		SetSlotColorLeft(slot_index, NANode.SignalColor(SignalType.Frequency));
		SetSlotTypeLeft(slot_index, (int)SignalType.Frequency);
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
			var input = GetChild<SpinBox>(_sequenceIndex);
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
	protected override ISampleProvider GetOutput(int port)
	{
		var slot = GetConnectionOutputSlot(port);
		if (slot != SequenceOutput) throw new IndexOutOfRangeException("Invalid output slot.");
		return _constant;
	}
}