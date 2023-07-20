using Godot;
using System;
using NAudio.Wave;
using SonicThinking.scripts.autoload;
using SonicThinking.scripts.nodes;
using SonicThinking.scripts.sample_providers;

public partial class NALoggerNode : NANode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_display = GetNode<WaveDisplay>("Display");
		_display.Data = new float[(int)(5.0 * NANode.DefaultWaveFormat.SampleRate)];

		Compositor.ForceCache += OnRead;
		
		InputChanged += OnInputChanged;

		_period = GetNode<SpinBox>("Period/SpinBox");
		_period.ValueChanged += ChangePeriod;
		ChangePeriod(_period.Value);
	}

	public override void _ExitTree()
	{
		Compositor.ForceCache -= OnRead;
	}

	private void ChangePeriod(double value)
	{
		var sampleCount = Mathf.FloorToInt(NANode.DefaultWaveFormat.SampleRate * value);
		var oldCount = _display.Data.Length;
		Array.Resize(ref _display.Data, sampleCount);
	}

	private void OnInputChanged(NANode sender, int portIndex, ISampleProvider input)
	{
		_rebinding.Source = input;
	}

	private void OnRead(int offset, int count)
	{
		var buffer = new float[count];
		_rebinding.Read(buffer, 0, count);
		_display.Push(buffer);
		_display.QueueRedraw();
	}
	
	protected override ISampleProvider GetOutput(int port)
	{
		throw new NotImplementedException();
	}

	private SpinBox _period;
	private WaveDisplay _display;
	private readonly RebindingProvider _rebinding = new RebindingProvider();
}
