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
	public String Label
	{
		get => _label.Text;
		set => _label.Text = value;
	}

	[Export]
	public float Value
	{
		get => float.Parse(_display.Text);
		set => _display.Text = value.ToString(CultureInfo.InvariantCulture);
	}
	
	private Label _label;
	private LineEdit _display;
}