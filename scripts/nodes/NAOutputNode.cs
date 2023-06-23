using System;
using Godot;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SonicThinking.scripts.autoload;
using SonicThinking.scripts.helpers;
using SonicThinking.scripts.sample_providers;

namespace SonicThinking.scripts.nodes;

public partial class NAOutputNode : NANode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_volume = new VolumeSampleProvider(_rebind);
		GetNode<Compositor>("/root/Compositor").AddOutput(_volume);

		_volume.Volume = (float)(VolumeSlider.Value / 100.0);
		VolumeSlider.ValueChanged += value => _volume.Volume = (float)(value / 100.0);
		InputChanged += OnInputChanged;
	}

	public override void _ExitTree()
	{
		GetNode<Compositor>("/root/Compositor").RemoveOutput(_volume);
		base._ExitTree();
	}

	private void OnInputChanged(NANode sender, int slot, ISampleProvider input)
	{
		_rebind.Source = input;
	}

	protected override ISampleProvider GetOutput(int port)
	{
		// This node does not provide outputs.
		throw new System.NotImplementedException();
	}

	private SliderInput VolumeSlider => GetNodeOrNull<SliderInput>("Volume");
	private readonly RebindingProvider _rebind = new RebindingProvider();
	private VolumeSampleProvider _volume;
}
