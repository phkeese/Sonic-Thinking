using System;
using System.Collections.Generic;
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
		_timingBox = GetNode<SpinBox>("%TimingBox");
		_removeButton = GetNode<Button>("%Remove");
		
		foreach (var child in GetChildren())
		{
			if (child.IsInGroup("internal"))
			{
				RemoveChild(child);
				AddChild(child, false, InternalMode.Front);
			}
		}
		AddStep();
		
		InputChanged += OnInputChanged;
		Compositor.ForceCache += _cache.Force;
		Compositor.ClearCache += _cache.Clear;
		
		_timingBox.ValueChanged += value => _constantIndex.Value = (float)value;
		_constantIndex.Value = (float)_timingBox.Value;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		Compositor.ForceCache -= _cache.Force;
		Compositor.ClearCache -= _cache.Clear;
	}

	private void OnInputChanged(NANode sender, int portIndex, ISampleProvider input)
	{
		if (portIndex == 0)
		{
			_index.Source = input;
		}
		else if(portIndex == 1)
		{
			GD.Print($"Set interval input to {input}");
		}
		else
		{
			var stepIndex = portIndex - 2;
			_steps[stepIndex].Override = input;
		}
	}

	public void Skip(int steps)
	{
		GD.Print($"Skipping {steps} steps.");
	}

	private void AddStep()
	{
		
		var sequenceStepScene = GD.Load<PackedScene>("res://scenes/helpers/sequence_step.tscn");
		var step = sequenceStepScene.Instantiate<SequenceStep>();

		_steps.Add(step);
		_mux.Add(step);
		
		var slotIndex = GetChildCount(true);
		AddChild(step);
		
		SetSlotEnabledLeft(slotIndex, true);
		SetSlotColorLeft(slotIndex, SignalColor(SignalType.Any));
		SetSlotTypeLeft(slotIndex, (int)SignalType.Any);

		_removeButton.Disabled = GetChildCount() == 1;
	}

	private void RemoveStep()
	{
		if (GetChildCount() == 0)
			return;
		var step = GetChild<SequenceStep>(GetChildCount() - 1);
		RemoveChild(step);
		step.QueueFree();

		_steps.Remove(step);
		_mux.Remove(step);


		if (GetChildCount() == 1)
		{
			_removeButton.Disabled = true;
		}
	}


	public NASequencerNode()
	{
		_index = new PrioritySampleProvider(_constantIndex);
		_mux = new RingMuxSampleProvider(_index);
		_cache = new CachingSampleProvider(_mux);
	}

	private const int SequenceOutput = 0;
	public override Dictionary Serialize()
	{
		return new Dictionary()
		{
			{ "timing", _timingBox.Value },
			{ "steps", GetStepValues() },
		};
	}

	private double[] GetStepValues()
	{
		var steps = new double[GetChildCount()];
		for (int i = 0; i < GetChildCount(); i++)
		{
			steps[i] = (float)GetChild(i).Get("Value");
		}

		return steps;
	}

	public override void Deserialize(Dictionary state)
	{
		_timingBox.Value = state["timing"].AsDouble();
		
		// Ensure there are enough steps
		var stepValues = state["steps"].AsFloat64Array();
		while (GetChildCount() < stepValues.Length)
		{
			AddStep();
		}

		for (int i = 0; i < stepValues.Length; i++)
		{
			_steps[i].Value = stepValues[i];
		}
	}

	protected override ISampleProvider GetOutput(int port) => _cache;

	private readonly PrioritySampleProvider _index;
	private readonly ConstantSampleProvider _constantIndex = new ConstantSampleProvider();
	private readonly RingMuxSampleProvider _mux;
	private readonly CachingSampleProvider _cache;

	private readonly List<SequenceStep> _steps = new List<SequenceStep>();
	private SpinBox _timingBox;
	private Button _removeButton;
	
	private double _time = 0.0;
	private double _nextTrigger = 0.0;
	private int _sequenceIndex = 0;
}
