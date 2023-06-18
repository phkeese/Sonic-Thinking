using System;
using Godot;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SonicThinking.scripts.nodes;

public partial class NAVolumeNode : NANode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_volumeSpinBox = GetNode<SpinBox>("Volume/SpinBox");
		_volumeSlider = GetNode<HSlider>("VolumeSlider");

		_volumeSpinBox.ValueChanged += OnVolumeChanged;
		_volumeSlider.ValueChanged += OnVolumeChanged;

		InputChanged += OnInputChanged;
	}

	private void OnInputChanged(NANode sender, int slotIndex, ISampleProvider input)
	{
		// FIXME: Changing the input while an output is connected does not do anything until that output is also
		// reconnected. Maybe a custom class for volume which can switch the input on the fly?
		if (slotIndex != InputSlot) throw new IndexOutOfRangeException("Invalid input slot.");
		if (input != null)
		{
			_provider = new VolumeSampleProvider(input);
		}
		else
		{
			_provider = null;
		}
	}

	private void OnVolumeChanged(double value)
	{
		if (_provider == null) return;
		_provider.Volume = (float)(value / 100f);
		
		_volumeSlider.Set("value", value);
		_volumeSpinBox.Set("value", value);
	}
	
	protected override ISampleProvider GetOutput(int port)
	{
		var slot = GetConnectionInputSlot(port);
		if (slot != OutputSlot) throw new IndexOutOfRangeException("Invalid output slot.");
		return _provider;
	}

	private VolumeSampleProvider _provider;
	private SpinBox _volumeSpinBox;
	private HSlider _volumeSlider;

	public const int InputSlot = 0;
	public const int OutputSlot = 0;
}