using Godot;
using NAudio.Wave;
using SonicThinking.scripts.nodes;
using SonicThinking.scripts.sample_providers;

namespace SonicThinking.nodes;

public partial class NAGateNode : NANode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var button = GetNode<Button>("Trigger");
		button.Toggled += pressed => _gate.Value = pressed ? 1 : 0;
	}

	protected override ISampleProvider GetOutput(int port)
	{
		return _gate;
	}

	private readonly ConstantSampleProvider _gate = new ConstantSampleProvider();
}