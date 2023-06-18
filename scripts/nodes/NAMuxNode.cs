using Godot;
using System;
using NAudio.Wave;
using SonicThinking.scripts.nodes;
using SonicThinking.scripts.sample_providers;

/// <summary>
/// Allows quick switching between a number of inputs.
/// </summary>
public partial class NAMuxNode : NANode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_selectA = GetNode<CheckButton>("A");
		_selectB = GetNode<CheckButton>("B");

		_selectA.Toggled += pressed =>
		{
			if (pressed)
			{
				_selectB.ButtonPressed = false;
			}
		};
	}

	protected override ISampleProvider GetOutput(int port)
	{
		var slot = GetConnectionOutputSlot(port);
		if (slot != OutputSlot) throw new IndexOutOfRangeException("Invalid output slot.");
		return _rebinding;
	}

	private RebindingProvider _rebinding = new RebindingProvider();
	private CheckButton _selectA;
	private CheckButton _selectB;
	
	public const int InputASlot = 0;
	public const int InputBSlot = 1;
	public const int OutputSlot = 0;
}
