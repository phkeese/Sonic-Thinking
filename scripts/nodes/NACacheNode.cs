using Godot.Collections;
using NAudio.Wave;
using SonicThinking.scripts.autoload;
using SonicThinking.scripts.sample_providers;

namespace SonicThinking.scripts.nodes;

public partial class NACacheNode : NANode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Compositor.ForceCache += _cache.Force;
		Compositor.ClearCache += _cache.Clear;

		InputChanged += (sender, index, input) => _rebinding.Source = input;
	}

	public override void _ExitTree()
	{
		Compositor.ForceCache -= _cache.Force;
		Compositor.ClearCache -= _cache.Clear;
	}

	protected override ISampleProvider GetOutput(int port)
	{
		return _cache;
	}

	private readonly RebindingProvider _rebinding = new RebindingProvider();
	private readonly CachingSampleProvider _cache;

	public NACacheNode()
	{
		_cache = new CachingSampleProvider(_rebinding);
	}
}