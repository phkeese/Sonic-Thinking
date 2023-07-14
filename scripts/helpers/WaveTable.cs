using Godot;
using System;
using SonicThinking.scripts.nodes;

public partial class WaveTable : Panel
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _Draw()
	{
		base._Draw();

		float height = GetRect().Size.Y;
		float centerline = height / 2.0f;
		float width = GetRect().Size.X;
		float step = 1.0f / width;

		DrawLine(new Vector2(0,centerline), new Vector2(width, centerline), Colors.LightGray);
		Vector2 last = new Vector2(0.0f, centerline);

		for (int x = 1; x < width; x++)
		{
			float tau = x / width;
			float y = Sample(tau);
			
			Vector2 here = new Vector2(x, -y * centerline + centerline);
			DrawLine(last, here, Colors.Red, 1f, true);
			last = here;
		}
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton button)
		{
			if (button.ButtonIndex == MouseButton.Left)
			{
				_isDrawing = button.IsPressed();
				if (_isDrawing)
				{
					_lastPosition = button.Position;
					DrawSample(button.Position);
				}
			}

			return;
		}
		else if (@event is InputEventMouseMotion motion)
		{
			if (!_isDrawing) return;
			DrawWave(_lastPosition, motion.Position);
			_lastPosition = motion.Position;
		}
	}

	private void DrawSample(Vector2 position)
	{
		DrawWave(position, position + Vector2.Right);
	}

	private void DrawWave(Vector2 left, Vector2 right)
	{
		if (left.X > right.X) (left, right) = (right, left);

		var width = GetRect().Size.X;
		var height = GetRect().Size.Y;

		var leftX = Mathf.Remap(left.X, 0, width, 0, 1);
		var rightX = Mathf.Remap(right.X, 0, width, 0, 1);
		
		var leftIndex = Mathf.FloorToInt(leftX * Wave.Length);
		var rightIndex = Mathf.CeilToInt(rightX * Wave.Length);

		leftIndex = Mathf.Max(leftIndex, 0);
		rightIndex = Mathf.Min(rightIndex, Wave.Length);
		var deltaI = rightIndex - leftIndex;


		var startY = Mathf.Clamp(Mathf.Remap(left.Y, 0f, height, 1f, -1f),-1,1);
		var endY = Mathf.Clamp(Mathf.Remap(right.Y, 0f, height, 1f, -1f),-1,1);
		var tStep = 1f / deltaI;

		for (int i = 0; i < deltaI; i++)
		{
			var y = Mathf.Lerp(startY, endY, i * tStep);
			Wave[leftIndex + i] = y;
		}
		
		QueueRedraw();
	}

	public float[] Wave;

	public WaveTable()
	{
		int sampleRate = NANode.DefaultWaveFormat.SampleRate;
		Wave = new float[sampleRate];
        
		Wave = new float[sampleRate];
		for (int index = 0; index < sampleRate; ++index)
		{
			Wave[index] = (float)Math.Sin(2 * Math.PI * (double)index / sampleRate);
			// For sawtooth instead of sine: waveTable[index] = (float)index / sampleRate;
		}
	}

	public float Sample(float phase)
	{
		float index = Wave.Length * phase;
		int leftIndex = Mathf.Max(Mathf.FloorToInt(index), 0);
		int rightIndex = Mathf.Min(Mathf.CeilToInt(index),Wave.Length - 1);
		
		return (Wave[leftIndex] + Wave[rightIndex]) / 2f;
	}

	private bool _isDrawing = false;
	private Vector2 _lastPosition;
}
