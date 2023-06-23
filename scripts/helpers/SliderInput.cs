using Godot;

namespace SonicThinking.scripts.helpers;

[Tool]
public partial class SliderInput : Control
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Begin Ready");
        Slider.ValueChanged += value => Value = value;
        SBox.ValueChanged += value => Value = value;
        GD.Print("SliderInput ready!");
    }

    [Export]
    public string Label
    {
        get => LabelNode.Text;
        set => LabelNode.Text = value;
    }

    [Export]
    public double MaxValue
    {
        get => SBox.MaxValue;
        set => SBox.MaxValue = Slider.MaxValue = value;
    }

    [Export]
    public double MinValue
    {
        get => SBox.MinValue;
        set => SBox.MinValue = Slider.MinValue = value;
    }

    [Export]
    public double Value
    {
        get => SBox.Value;
        set => SBox.Value = Slider.Value = value;
    }

    [Export]
    public double Step
    {
        get => SBox.Step;
        set => SBox.Step = Slider.Step = value;
    }

    [Export]
    public bool ExpEdit
    {
        get => SBox.ExpEdit;
        set => SBox.ExpEdit = Slider.ExpEdit = value;
    }

    [Export]
    public string Suffix
    {
        get => SBox.Suffix;
        set => SBox.Suffix = value;
    }

    private Label LabelNode => GetNode<Label>("TopRow/Label");
    private Slider Slider => GetNode<Slider>("Slider");

    private SpinBox SBox => GetNode<SpinBox>("TopRow/SpinBox");
}