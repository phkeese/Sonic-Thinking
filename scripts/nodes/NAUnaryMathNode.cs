using Godot;
using System;
using NAudio.Wave;
using SonicThinking.scripts.nodes;
using SonicThinking.scripts.sample_providers;

public partial class NAUnaryMathNode : NANode, ISampleProvider
{
	public override void _Ready()
	{
		base._Ready();

		_operationSelect = GetNode<OptionButton>("%Operation");
		_operationSelect.ItemSelected += index => _operation = (Operation)index;

		InputChanged += (_, index, input) =>
		{
			switch (index)
			{
				case 0:
					_leftInput.Source = input;
					break;
				default:
					throw new ArgumentException();
			}
		};

		Compositor.ForceCache += _cache.Force;
		Compositor.ClearCache += _cache.Clear;
	}

	public override void _ExitTree()
	{
		Compositor.ForceCache -= _cache.Force;
		Compositor.ClearCache -= _cache.Clear;
	}

	protected override ISampleProvider GetOutput(int port) => _cache;

	private enum Operation
	{
		Negate,
		Floor,
		Ceil,
		Abs,
		Sqrt,
		Round,
		Exp,
		Log,
		Fraction,
	}

	private OptionButton _operationSelect;
	private Operation _operation = Operation.Negate;
	private readonly RebindingProvider _leftInput = new(), _rightInput = new();
	private readonly CachingSampleProvider _cache;

	public NAUnaryMathNode()
	{
		_cache = new CachingSampleProvider(this);
	}

	public int Read(float[] buffer, int offset, int count)
	{
		var leftValues = new float[buffer.Length];
		_leftInput.Read(leftValues, offset, count);

		for (int i = 0; i < count; i++)
		{
			var left = leftValues[offset + i];

			buffer[offset + i] = _operation switch
			{
				Operation.Negate => -left,
				Operation.Floor => Mathf.Floor(left),
				Operation.Ceil => Mathf.Ceil(left),
				Operation.Abs => Mathf.Abs(left),
				Operation.Sqrt => Mathf.Sqrt(left),
				Operation.Round => Mathf.Round(left),
				Operation.Exp => Mathf.Exp(left),
				Operation.Log => Mathf.Log(left),
				Operation.Fraction => left - Mathf.Floor(left),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		return count;
	}

	public WaveFormat WaveFormat => DefaultWaveFormat;
}
