using Godot;
using NAudio.Wave;
using SonicThinking.scripts.sample_providers;

namespace SonicThinking.scripts.nodes;

public partial class NaRemapNode : NANode, ISampleProvider
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_fromMin = GetNode<SpinBox>("%FromMin");
		_fromMax = GetNode<SpinBox>("%FromMax");
		_toMin = GetNode<SpinBox>("%ToMin");
		_toMax = GetNode<SpinBox>("%ToMax");
		_clamp = GetNode<CheckBox>("Clamp");

		InputChanged += (sender, index, input) => _input.Source = input;
	}

	protected override ISampleProvider GetOutput(int port) => this;
	
	public int Read(float[] buffer, int offset, int count)
	{
		var total = _input.Read(buffer, offset, count);
		for (int i = 0; i < total; i++)
		{
			var value = Mathf.Remap(buffer[offset + i], _fromMin.Value, _fromMax.Value, _toMin.Value, _toMax.Value);
			buffer[offset + i] = (float)(_clamp.ButtonPressed ? Mathf.Clamp(value, _toMin.Value, _toMax.Value) : value);
		}

		return total;
	}

	public WaveFormat WaveFormat => DefaultWaveFormat;

	private readonly RebindingProvider _input = new();
	private SpinBox _fromMin;
	private SpinBox _fromMax;
	private SpinBox _toMin;
	private SpinBox _toMax;
	private CheckBox _clamp;
}