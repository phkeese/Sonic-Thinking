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
		_addButton = GetNode<Button>("AddButton");

		_addButton.Pressed += AddPart;
	}

	private void AddPart()
	{
		var FrequencyEdit = GD.Load<PackedScene>("res://scenes/helpers/frequency_edit.tscn");
		var instance = FrequencyEdit.Instantiate<SpinBox>();
		AddChild(instance);
		GD.Print("Yes");
		MoveChild(instance, _sequenceEndIndex++);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_time += delta;
		if (_nextTrigger <= _time)
		{
			int sequenceLength = _sequenceEndIndex - SequenceStartIndex;
			_sequenceIndex = (_sequenceIndex + 1) % sequenceLength;
			var input = GetChild<SpinBox>(SequenceStartIndex + _sequenceIndex);
			_constant.Value = (float)input.Value;
			_nextTrigger = _time + _timingBox.Value;
		}
	}

	private readonly ConstantSampleProvider _constant = new ConstantSampleProvider();
	private SpinBox _timingBox;
	private Button _addButton;
	private const int SequenceStartIndex = 1;
	private int _sequenceEndIndex = SequenceStartIndex + 1;
	private int _sequenceIndex = 0;
	private double _time = 0.0;
	private double _nextTrigger = 0.0;

	private const int SequenceOutput = 0;
	protected override ISampleProvider GetOutput(int port)
	{
		var slot = GetConnectionOutputSlot(port);
		if (slot != SequenceOutput) throw new IndexOutOfRangeException("Invalid output slot.");
		return _constant;
	}
}