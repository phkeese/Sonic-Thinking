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
    #endregion

    #region Signals

    [Signal]
    public delegate void ValueChangedEventHandler(double value);

    #endregion

    private Label LabelNode => GetNodeOrNull<Label>("TopRow/Label");
    private Slider Slider => GetNodeOrNull<Slider>("Slider");

    private SpinBox SBox => GetNodeOrNull<SpinBox>("TopRow/SpinBox");
}