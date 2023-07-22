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
			{ "total", _counter.Count }
		};
	}

	public override void Deserialize(Dictionary state)
	{
		_counter.Count = state["total"].AsInt32();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_display = GetNode<SpinBox>("Display");
		_display.ValueChanged += value => _counter.Count = (int)value;

		var reset = GetNode<Button>("Reset");
		reset.Pressed += () => _counter.Count = 0;
		InputChanged += (sender, index, input) =>
		{
			_rebinding.Source = input;
			_display.Editable = input == null;
		};
		
		Compositor.ForceCache += _cache.Force;
		Compositor.ClearCache += _cache.Clear;
		_counter.Changed += count => _display.Value = count;
	}
	
	public override void _ExitTree()
	{
		Compositor.ForceCache -= _cache.Force;
		Compositor.ClearCache -= _cache.Clear;
	}

	protected override ISampleProvider GetOutput(int port) => _cache;

	private SpinBox _display;
	private readonly RebindingProvider _rebinding = new RebindingProvider();
	private readonly CountingSampleProvider _counter;
	private readonly CachingSampleProvider _cache;

	public NACounterNode()
	{
		_counter = new CountingSampleProvider(_rebinding);
		_cache = new CachingSampleProvider(_counter);
	}
}