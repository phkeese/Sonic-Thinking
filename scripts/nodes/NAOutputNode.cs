using System;
using Godot;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SonicThinking.scripts.sample_providers;

namespace SonicThinking.scripts.nodes;

public partial class NAOutputNode : NANode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_notify.Source = _rebinding;
		_wo.Init(_notify);
		_wo.Play();

		_debugLed = GetNode<TextureRect>("TextureRect");
		_notify.PostRead += (buffer, offset, count) => _debugLed.Visible = !_debugLed.Visible;
		_notify.PostRead += (buffer, offset, count) => GD.Print("Read!");
		
		InputChanged += OnInputChanged;
	}

	private void OnInputChanged(NANode sender, int slot, ISampleProvider input)
	{
		if (slot != InputSlot) throw new IndexOutOfRangeException("Invalid slot index.");
		_rebinding.Source = input;
	}

	protected override ISampleProvider GetOutput(int port)
	{
		// This node does not provide outputs.
		throw new System.NotImplementedException();
	}
	
	/// <summary>
	/// Input slot to get samples from.
	/// </summary>
	private const int InputSlot = 0;
	
	private readonly WaveOutEvent _wo = new WaveOutEvent();
	private readonly RebindingProvider _rebinding = new RebindingProvider();
	private readonly NotifyOnReadProvider _notify = new NotifyOnReadProvider();

	private TextureRect _debugLed;
}
