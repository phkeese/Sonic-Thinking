using System;
using System.Globalization;
using Godot;

namespace SonicThinking.scripts.helpers;

[Tool]
public partial class ValueDisplay : HBoxContainer
{
    public override void _Ready()
    {
        base._Ready();

        _label = GetNode<Label>("Label");
        _display = GetNode<LineEdit>("Display");
    }

    [Export]
    public string Label
    {
        get => _label != null ? _label.Text : "";
        set
        {
            if (_label != null) _label.Text = value;
        }
    }

    [Export]
    public float Value
    {
        get => _display != null ? float.Parse(_display.Text) : 0.0f;
        set
        {
            if (_display != null) _display.Text = value.ToString(CultureInfo.InvariantCulture);
        }
    }

    private Label _label;
    private LineEdit _display;
}