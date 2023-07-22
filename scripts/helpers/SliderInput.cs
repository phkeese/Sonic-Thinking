using Godot;

namespace SonicThinking.scripts.helpers;

[Tool]
public partial class SliderInput : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Slider.ValueChanged += value => Value = value;
		SBox.ValueChanged += value => Value = value;
	}

	#region Exports
	[Export]
	public string Label
	{
		get => LabelNode.Text;
		set
		{
			if (LabelNode != null) LabelNode.Text = value;
		}
	}

	[Export]
	public double MaxValue
	{
		get => SBox.MaxValue;
		set
		{
			if (SBox != null) SBox.MaxValue = value;
			if (Slider != null) Slider.MaxValue = value;
		}
	}

	[Export]
	public double MinValue
	{
		get => SBox.MinValue;
		set
		{
			if (SBox != null) SBox.MinValue = value;
			if (Slider != null) Slider.MinValue = value;
		}
	}

	[Export]
	public double Value
	{
		get => SBox.Value;
		set
		{
			if (SBox != null) SBox.Value = value;
			if (Slider != null) Slider.Value = value;
			EmitSignal(SignalName.ValueChanged, value);
		}
	}

	[Export]
	public double Step
	{
		get => SBox.Step;
		set
		{
			if (SBox != null) SBox.Step = value;
			if (Slider != null) Slider.Step = value;
		}
	}

	[Export]
	public bool ExpEdit
	{
		get => SBox.ExpEdit;
		set
		{
			if (SBox != null) SBox.ExpEdit = value;
			if (Slider != null) Slider.ExpEdit = value;
		}
	}

	[Export]
	public bool AllowGreater
	{
		get => SBox.AllowGreater;
		set
		{
			if (SBox != null) SBox.AllowGreater = value;
			if (Slider!= null) Slider.AllowGreater = value;
		}
	}
	
	
	[Export]
	public bool AllowLesser
	{
		get => SBox.AllowLesser;
		set
		{
			if (SBox != null) SBox.AllowLesser = value;
			if (Slider!= null) Slider.AllowLesser = value;
		}
	}

	[Export]
	public string Suffix
	{
		get => SBox.Suffix;
		set
		{
			if (SBox != null) SBox.Suffix = value;
		}
	}

	[Export]
	public int TickCount
	{
		get => Slider.TickCount;
		set
		{
			if (Slider != null) Slider.TickCount = value;
		}
	}

	[Export]
	public bool TicksOnBorders
	{
		get => Slider.TicksOnBorders;
		set
		{
			if (Slider != null) Slider.TicksOnBorders = value;
		}
	}

	[Export]
	public bool Editable
	{
		get => SBox.Editable;
		set
		{
			if (SBox != null) SBox.Editable = value;
			if (Slider != null) Slider.Editable = value;
		}
	}
	#endregion

	#region Signals

	[Signal]
	public delegate void ValueChangedEventHandler(double value);

	#endregion

	private Label LabelNode => GetNodeOrNull<Label>("TopRow/Label");
	private Slider Slider => GetNodeOrNull<Slider>("Slider");

	private SpinBox SBox => GetNodeOrNull<SpinBox>("TopRow/SpinBox");
}
