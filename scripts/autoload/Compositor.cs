using System.Collections.Generic;
using Godot;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SonicThinking.scripts.helpers;
using SonicThinking.scripts.nodes;
using SonicThinking.scripts.sample_providers;

namespace SonicThinking.scripts.autoload;

public partial class Compositor : Node
{
	public override void _Ready()
	{
		base._Ready();

		_mixer.ReadFully = true;

		_notify = new ReadNotifyProvider(_mixer);
		_notify.OnRead += AfterRead;
		
		_waveOut.Init(_notify);
		_waveOut.Play();
	}

	private void AfterRead(float[] buffer, int offset, int count, int result)
	{
		EmitSignal(SignalName.ForceCache,  offset, count);
		EmitSignal(SignalName.ClearCache);
	}

	public void AddOutput(ISampleProvider source)
	{
		_mixer.AddMixerInput(source);
	}

	public void RemoveOutput(ISampleProvider source)
	{
		_mixer.RemoveMixerInput(source);
	}

	#region Signals

	/// <summary>
	/// Emitted when a new buffer of audio data has been filled.
	/// All caches should call Read(count, offset) on their inputs now.
	/// </summary>
	[Signal]
	public delegate void ForceCacheEventHandler(int offset, int count);

	/// <summary>
	/// Emitted to force caches to clear their read contents.
	/// </summary>
	[Signal]
	public delegate void ClearCacheEventHandler();

	#endregion

	#region NAudio

	private readonly MixingSampleProvider _mixer = new MixingSampleProvider(NANode.DefaultWaveFormat);
	private ReadNotifyProvider _notify;
	private readonly WaveOutEvent _waveOut = new WaveOutEvent();

	#endregion
}