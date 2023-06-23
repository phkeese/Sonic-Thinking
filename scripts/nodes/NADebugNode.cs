using Godot;
using NAudio.Wave;
using SonicThinking.scripts.helpers;
using SonicThinking.scripts.sample_providers;

namespace SonicThinking.scripts.nodes;

public partial class NADebugNode : NANode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_value = GetNode<ValueDisplay>("Value");
		_average = GetNode<ValueDisplay>("Average");
		_led = GetNode<CanvasItem>("Trigger/LED");
		
		InputChanged += OnInputChanged;
	}

	private void OnInputChanged(NANode sender, int slot, ISampleProvider input)
	{
		var notify = new ReadNotifyProvider(input);
		notify.OnRead += OnRead;
		_rebinding.Source = notify;
	}

	private void OnRead(float[] buffer, int offset, int count, int result)
	{
		if (result == 0)
		{
			return;
		}
		
		var last = buffer[result - 1];
		_led.Modulate = Colors.Red * last;
		_value.Value = last;
		GD.Print(last);
	}

	protected override ISampleProvider GetOutput(int port) => _rebinding;

	private readonly RebindingProvider _rebinding = new RebindingProvider();
	private ValueDisplay _value;
	private ValueDisplay _average;
	private CanvasItem _led;
}
