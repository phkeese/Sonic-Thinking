using Godot;
using System;
using NAudio.Wave;
using SonicThinking.scripts.nodes;

public partial class NaManualTriggerNode : NANode, ISampleProvider
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_trigger = GetNode<Button>("Button");
		_trigger.Pressed += () => _shouldTrigger = true;
	}

	protected override ISampleProvider GetOutput(int port) => this;

	public int Read(float[] buffer, int offset, int count)
	{
		if (count < 1) return count;
		buffer[offset] = _shouldTrigger ? 1 : 0;
		_shouldTrigger = false;
		return count;
	}

	public WaveFormat WaveFormat => DefaultWaveFormat;

	private bool _shouldTrigger = false;
	private Button _trigger;
}
