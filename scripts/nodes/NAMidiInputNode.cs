using Godot;
using System;
using NAudio.Midi;
using NAudio.Wave;
using SonicThinking.scripts.nodes;
using SonicThinking.scripts.sample_providers;

public partial class NAMidiInputNode : NANode
{
	private OptionButton _deviceSelect;
	private MidiIn _midiIn;
	private int _lastNote = -1;
	public override void _Ready()
	{
		_deviceSelect = GetNode<OptionButton>("%DeviceSelect");
		
		RegisterDevices();

		_deviceSelect.Selected = -1;
		_deviceSelect.ItemSelected += SelectDevice;
	}

	private void RegisterDevices()
	{
		for (int deviceNumber = 0; deviceNumber < MidiIn.NumberOfDevices; deviceNumber++)
		{
			var device = MidiIn.DeviceInfo(deviceNumber);
			_deviceSelect.AddItem(device.ProductName);
			GD.Print(device.ProductName);
		}
	}

	private void SelectDevice(long index)
	{
		if (_midiIn != null)
		{
			_midiIn.Stop();
			_midiIn.ErrorReceived -= MidiErrorReceived;
			_midiIn.MessageReceived -= MidiMessageReceived;
		}

		var device = MidiIn.DeviceInfo((int)index);
		GD.Print($"Switching to {device.ProductName}");
		
		_midiIn = new MidiIn((int)index);
		_midiIn.MessageReceived += MidiMessageReceived;
		_midiIn.ErrorReceived += MidiErrorReceived;
		_midiIn.Start();
	}

	private void MidiMessageReceived(object sender, MidiInMessageEventArgs e)
	{
		GD.Print($"MIDI Message: {e.MidiEvent}");
		var midiEvent = e.MidiEvent;

		if (midiEvent is NoteOnEvent noteOn && _lastNote == -1)
		{
			GD.Print($"Note {noteOn.NoteName} ({noteOn.NoteNumber}) turned on at {noteOn.Velocity}");

			_lastNote = noteOn.NoteNumber;
			_pitch.Value = ResolvePitch(noteOn.NoteNumber);
			_velocity.Value = noteOn.Velocity / 127f;
		} else if (midiEvent is NoteEvent noteOff && noteOff.NoteNumber == _lastNote)
		{
			GD.Print($"Note {noteOff.NoteName} ({noteOff.NoteNumber}) turned off");

			_velocity.Value = 0;
			_lastNote = -1;
		}
	}

	private float ResolvePitch(int noteNumber)
	{
		var frequency = Mathf.Pow(2, (noteNumber - 69) / 12f) * 440;
		return NANode.FrequencyToVoltage(frequency);
	}

	private void MidiErrorReceived(object sender, MidiInMessageEventArgs e)
	{
		GD.Print($"MIDI Errors: {e.MidiEvent}");
	}

	protected override ISampleProvider GetOutput(int port) => port switch { 0 => _pitch, 1 => _velocity,
		_ => throw new ArgumentOutOfRangeException(nameof(port), port, null)
	};

	private readonly ConstantSampleProvider _pitch = new();
	private readonly ConstantSampleProvider _velocity = new();
}
