using System.Collections.Generic;
using Godot;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SonicThinking.scripts.nodes;
namespace SonicThinking.scripts.autoload;

public partial class Compositor : Node
{
	public override void _Ready()
	{
		base._Ready();

		_mixer.ReadFully = true;
		
		_waveOut.Init(_mixer);
		_waveOut.Play();
	}

	public void AddOutput(ISampleProvider source)
	{
		_mixer.AddMixerInput(source);
	}

	public void RemoveOutput(ISampleProvider source)
	{
		_mixer.RemoveMixerInput(source);
	}


	#region NAudio

	private readonly MixingSampleProvider _mixer = new MixingSampleProvider(NANode.DefaultWaveFormat);
	private readonly WaveOutEvent _waveOut = new WaveOutEvent();

	#endregion
}