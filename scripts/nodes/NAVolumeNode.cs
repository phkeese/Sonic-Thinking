using System;
using Godot;
using Godot.Collections;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SonicThinking.scripts.sample_providers;

namespace SonicThinking.scripts.nodes;

public partial class NAVolumeNode : NANode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_volume = new VolumeSampleProvider(_rebindingProvider);
		
		_volumeSpinBox = GetNode<SpinBox>("Volume/SpinBox");
		_volumeSlider = GetNode<HSlider>("VolumeSlider");

		_volumeSpinBox.ValueChanged += OnVolumeChanged;
		_volumeSlider.ValueChanged += OnVolumeChanged;

		_volumeSpinBox.Value = _volumeSpinBox.Value;
		_volumeSlider.Value = _volumeSpinBox.Value;

		InputChanged += OnInputChanged;
	}

	private void OnInputChanged(NANode sender, int slotIndex, ISampleProvider input)
	{
		// FIXME: Changing the input while an output is connected does not do anything until that output is also
		// reconnected. Maybe a custom class for volume which can switch the input on the fly?
		if (slotIndex != InputSlot) throw new IndexOutOfRangeException("Invalid input slot.");
		_rebindingProvider.Source = input;
	}

	private void OnVolumeChanged(double value)
	{
		if (_volume == null) return;
		_volume.Volume = (float)(value / 100f);
		
		_volumeSlider.Set("value", value);
		_volumeSpinBox.Set("value", value);
	}

	public override Dictionary Serialize()
	{
		return new Dictionary()
		{
			{ "volume", _volumeSlider.Value },
		};
	}

	public override void Deserialize(Dictionary state)
	{
		_volumeSlider.Value = state["volume"].AsDouble();
	}

	protected override ISampleProvider GetOutput(int port)
	{
		var slot = GetConnectionInputSlot(port);
		if (slot != OutputSlot) throw new IndexOutOfRangeException("Invalid output slot.");
		return _volume;
	}

	private readonly RebindingProvider _rebindingProvider = new RebindingProvider();
	private VolumeSampleProvider _volume;
	private SpinBox _volumeSpinBox;
	private HSlider _volumeSlider;

	public NAVolumeNode()
	{
		_volume = new VolumeSampleProvider(_rebindingProvider);
	}

	public const int InputSlot = 0;
	public const int OutputSlot = 0;
}