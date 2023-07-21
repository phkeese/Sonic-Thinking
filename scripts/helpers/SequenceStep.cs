using System;
using Godot;
using NAudio.Wave;
using SonicThinking.scripts.nodes;

namespace SonicThinking.scripts.helpers;

[Tool]
public partial class SequenceStep : HBoxContainer, ISampleProvider
{
    public int Read(float[] buffer, int offset, int count)
    {
        if (Override != null)
        {
            return Override.Read(buffer, offset, count);
        }


        for (int i = 0; i < count; i++)
        {
            buffer[offset + i] = (float)Value;
        }

        return count;
    }

    public WaveFormat WaveFormat => NANode.DefaultWaveFormat;


    public ISampleProvider Override = null;

    private Color _activeColor = Colors.Red;

    [Export]
    public Color ActiveColor
    {
        get => _activeColor;
        set
        {
            _activeColor = value;
            if (Engine.IsEditorHint() && Led != null) Led.Modulate = value;
        }
    }


    private Color _inactiveColor = Colors.Black;

    [Export]
    public Color InactiveColor
    {
        get => _inactiveColor;
        set
        {
            _inactiveColor = value;
            if (Engine.IsEditorHint() && Led != null) Led.Modulate = value;
        }
    }

    [Export]
    public bool Editable
    {
        get => Edit.Editable;
        set
        {
            if (Engine.IsEditorHint() && Edit != null) Edit.Editable = value;
        }
    }

    [Export]
    private double MinValue
    {
        get => Edit.MinValue;
        set
        {
            if (Edit != null) Edit.MinValue = value;
        }
    }

    [Export]
    private double MaxValue
    {
        get => Edit.MaxValue;
        set
        {
            if (Edit != null) Edit.MaxValue = value;
        }
    }


    [Export]
    public double Value
    {
        get => Edit.Value;
        set
        {
            if (Engine.IsEditorHint() && Edit != null) Edit.Value = value;
        }
    }

    private bool _active = false;

    [Export]
    public bool Active
    {
        get => _active;
        set
        {
            _active = value;
            if (Engine.IsEditorHint() && Led != null) Led.Modulate = value ? ActiveColor : InactiveColor;
        }
    }

    private SliderInput Edit => GetNodeOrNull<SliderInput>("Edit");
    private CanvasItem Led => GetNodeOrNull<CanvasItem>("LED");
}