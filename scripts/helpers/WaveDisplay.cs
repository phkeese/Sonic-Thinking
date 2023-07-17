using Godot;
using System;

public partial class WaveDisplay : Panel
{
	[Export] public float[] Data;

	public override void _Draw()
	{
		base._Draw();
		
		float height = GetRect().Size.Y;
		float centerline = height / 2.0f;
		float width = GetRect().Size.X;
		float step = 1.0f / width;
		float cursor = (float)_index / Data.Length * width;

		DrawLine(new Vector2(0,centerline), new Vector2(width, centerline), Colors.LightGray);
		DrawLine(new Vector2(cursor, 0), new Vector2(cursor, height), Colors.LightGray);
		
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

	private float Sample(float t)
	{
		var index = t * Data.Length;
		var left = Mathf.Clamp(Mathf.FloorToInt(index),0,Data.Length -1);
		var right = Mathf.Clamp(Mathf.CeilToInt(index),0,Data.Length - 1);
		return (Data[left] + Data[right]) / 2;
	}

	public void Push(float[] data)
	{
		foreach (var sample in data)
		{
			Append(sample);
		}
	}

	private int _index = 0;
	private void Append(float sample)
	{
		_index %= Data.Length;
		Data[_index] = sample;
		_index = (_index + 1) % Data.Length;
	}
}
