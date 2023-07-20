using System;
using Godot;
using Godot.Collections;
using NAudio.Wave;
using SonicThinking.scripts.sample_providers;

namespace SonicThinking.scripts.nodes;

public partial class NACounterNode : NANode
{
	public override Dictionary Serialize()
	{
		return new Dictionary()
		{
			{ "total", _total }
		};
	}

	public override void Deserialize(Dictionary state)
	{
		_total = state["total"].AsInt32();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_display = GetNode<SpinBox>("Display");
		GetNode<Button>("Reset").Pressed += () => _total = 0;
		InputChanged += (sender, index, input) => _rebinding.Source = input;
		Compositor.ForceCache += Count;
	}

	private void Count(int offset, int count)
	{
		var buffer = new float[offset + count];
		count = _rebinding.Read(buffer, offset, count);
		for (int i = 0; i < count; i++)
		{
			if (buffer[offset + i] > 0.5) _total++;
		}

		_display.Value = _total;
	}

	public override void _ExitTree()
	{
		Compositor.ForceCache -= Count;
	}

	protected override ISampleProvider GetOutput(int port)
	{
		throw new NotImplementedException();
	}

	private SpinBox _display;
	private readonly RebindingProvider _rebinding = new RebindingProvider();
	private int _total = 0;
}