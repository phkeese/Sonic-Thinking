using System;
using Godot;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SonicThinking.scripts.autoload;
using SonicThinking.scripts.helpers;
using SonicThinking.scripts.sample_providers;

namespace SonicThinking.scripts.nodes;

public partial class NASignalGeneratorNode : NANode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_waveTable = GetNode<WaveTable>("WaveTable");
		InputChanged += OnInputChanged;

		_generator.WaveTable = _waveTable.Wave;
	}

	private void OnInputChanged(NANode sender, int slotIndex, ISampleProvider input)
	{
		if (slotIndex != FrequencySlot) throw new IndexOutOfRangeException("Invalid input slot.");
		_frequencyProvider.Source = input;
	}

	protected override ISampleProvider GetOutput(int port)
	{
		return _generator;
	}

	private const int FrequencySlot = 0;

	private readonly RebindingProvider _frequencyProvider = new RebindingProvider();
	private readonly LUTSignalGenerator _generator;
	private WaveTable _waveTable;

	public NASignalGeneratorNode()
	{
		_generator = new LUTSignalGenerator(_frequencyProvider);
	}
}