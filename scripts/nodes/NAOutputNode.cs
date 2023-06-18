using System;
using Godot;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SonicThinking.scripts.nodes;

public partial class NAOutputNode : NANode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InputChanged += OnInputChanged;
	}

	private void OnInputChanged(NANode sender, int slot, ISampleProvider input)
	{
		if (slot != InputSlot) throw new IndexOutOfRangeException("Invalid slot index.");
		if (input == null)
		{
			_wo.Stop();
		}
		else
		{
			_wo.Init(input);
			_wo.Play();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	protected override ISampleProvider GetOutput(int port)
	{
		// This node does not provide outputs.
		throw new System.NotImplementedException();
	}
	
	/// <summary>
	/// Input slot to get samples from.
	/// </summary>
	public const int InputSlot = 0;
	
	private readonly WaveOutEvent _wo = new WaveOutEvent();
	private readonly SignalGenerator _fallback = new SignalGenerator();
}
