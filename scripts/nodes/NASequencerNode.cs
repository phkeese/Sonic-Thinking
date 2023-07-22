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
        _removeButton = GetNode<Button>("%Remove");
        _indexBox = GetNode<SpinBox>("%IndexBox");

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

        _indexBox.ValueChanged += value => _counter.Count = (int)value;
        _counter.Changed += count => _indexBox.Value = count;
        _counter.Count = (int)_indexBox.Value;
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
            _indexPrio.Source = input;
            _indexBox.Editable = input == null;
        }
        else if (portIndex == 1)
        {
            _triggerInput.Source = input;
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
        var half = new ConstantSampleProvider
        {
            Value = 0.5f
        };
        var triggerEdge = new EdgeDetectorProvider(source: _triggerInput, threshold: half);
        _counter = new CountingSampleProvider(trigger: triggerEdge);
        _indexPrio = new PrioritySampleProvider(fallback: _counter);

        _mux = new RingMuxSampleProvider(index: _indexPrio);
        _cache = new CachingSampleProvider(source: _mux);
    }

    private const int SequenceOutput = 0;

    public override Dictionary Serialize()
    {
        return new Dictionary()
        {
            { "index", _indexBox.Value },
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
        _indexBox.Value = state["index"].AsDouble();

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

    /*
     * Index Box-------------------------v
     * Trigger Input--->| Edge |--->| Counter |--->| Index Prio (fallback) |
     */
    private readonly RebindingProvider _triggerInput = new RebindingProvider();
    private readonly CountingSampleProvider _counter;

    // Index selection
    /*
     * | Index Counter |--->| Index Prio (fallback) |-->| Mux |
     * Index Input--------->| (override)            |
     */
    private readonly PrioritySampleProvider _indexPrio;
    private readonly ConstantSampleProvider _constantIndex = new ConstantSampleProvider();
    private readonly RingMuxSampleProvider _mux;
    private readonly CachingSampleProvider _cache;

    private readonly List<SequenceStep> _steps = new List<SequenceStep>();
    private Button _removeButton;
    private SpinBox _indexBox;

    private double _time = 0.0;
    private double _nextTrigger = 0.0;
    private int _sequenceIndex = 0;
}