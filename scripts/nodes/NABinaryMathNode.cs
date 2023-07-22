using Godot;
using System;
using NAudio.Wave;
using SonicThinking.scripts.nodes;
using SonicThinking.scripts.sample_providers;

public partial class NABinaryMathNode : NANode, ISampleProvider
{
	public override void _Ready()
	{
		base._Ready();

		_operationSelect = GetNode<OptionButton>("Operation");
		_operationSelect.ItemSelected += index => _operation = (Operation)index;

		InputChanged += (_, index, input) =>
		{
			switch (index)
			{
				case 0:
					_leftInput.Source = input;
					break;
				case 1:
					_rightInput.Source = input;
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
		Add = 0,
		Subtract = 1,
		Multiply = 2,
		Divide = 3,
		Modulo= 4,
	}

	private OptionButton _operationSelect;
	private Operation _operation = Operation.Add;
	private readonly RebindingProvider _leftInput = new(), _rightInput = new();
	private readonly CachingSampleProvider _cache;

	public NABinaryMathNode()
	{
		_cache = new CachingSampleProvider(this);
	}

	public int Read(float[] buffer, int offset, int count)
	{
		var leftValues = new float[buffer.Length];
		_leftInput.Read(leftValues, offset, count);
		
		var rightValues = new float[buffer.Length];
		_rightInput.Read(rightValues, offset, count);

		for (int i = 0; i < count; i++)
		{
			var left = leftValues[offset + i];
			var right = rightValues[offset + i];

			buffer[offset + i] = _operation switch
			{
				Operation.Add => left + right,
				Operation.Subtract => left - right,
				Operation.Multiply => left * right,
				Operation.Divide => left / right,
				Operation.Modulo => Mathf.PosMod(left, right),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		return count;
	}

	public WaveFormat WaveFormat => DefaultWaveFormat;
}
