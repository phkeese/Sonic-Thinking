using System;
using Godot;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SonicThinking.scripts.nodes;

public enum SignalType : int
{
	/// <summary>
	/// Audio signal. Red.
	/// </summary>
	Wave = 1,
	
	/// <summary>
	/// General Control Voltage. Green.
	/// When interpreted as a boolean, [0;0.5) is false and [0.5;1] is true.
	/// </summary>
	Gate = 2,
	
	/// <summary>
	/// Frequency signal in 1V/Octave. Yellow.
	/// </summary>
	Frequency = 3,
	
	/// <summary>
	/// Can take any input and output to anything. Purple.
	/// </summary>
	Any = 4,
}

public abstract partial class NANode : GraphNode
{
	/// <summary>
	/// Get the output at a given slot for this node.
	/// </summary>
	/// <param name="port">Index of the requested slot.</param>
	/// <returns>Output if there is any, null otherwise.</returns>
	protected abstract ISampleProvider GetOutput(int port);
	
	/// <summary>
	/// Connect an output from another node to this node's input.
	/// </summary>
	/// <param name="from">Node connection is made from.</param>
	/// <param name="fromPort">Port index the connection is made from.</param>
	/// <param name="toPort">Slot the connection is made to.</param>
	public void ConnectInput(NANode from, int fromPort, int toPort)
	{
		var input= from.GetOutput(fromPort);
		InputChanged?.Invoke(this, toPort, input);
	}

	/// <summary>
	/// Disconnect an output from another node to this node's input.
	/// </summary>
	/// <param name="from">Node connection was made from.</param>
	/// <param name="fromPort">Port index the connection was made from.</param>
	/// <param name="toPort">Slot the connection was made to.</param>
	public void DisconnectInput(NANode from, int fromPort, int toPort)
	{
		var slot = GetConnectionInputSlot(toPort);
		InputChanged?.Invoke(this, slot, null);
	}
	
	/// <summary>
	/// Handles a change in connections to this node.
	/// </summary>
	public delegate void InputChangedHandler(NANode sender, int portIndex, ISampleProvider input);
	
	
	/// <summary>
	/// Maps a SignalType to a Color
	/// </summary>
	/// <param name="signalType">SignalType to request color for.</param>
	/// <returns>Corresponding color.</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static Color SignalColor(SignalType signalType)
	{
		switch (signalType)
		{
			case SignalType.Wave:
				return Colors.Red;
			case SignalType.Gate:
				return Colors.Green;
			case SignalType.Frequency:
				return Colors.Yellow;
			case SignalType.Any:
				return Colors.Purple;
			default:
				throw new ArgumentOutOfRangeException(nameof(signalType), signalType, null);
		}
	}

	/// <summary>
	/// Fired when an input connection is made or broken.
	/// </summary>
	protected InputChangedHandler InputChanged;

	protected const int DefaultSampleRate = 41_000;
	protected const int DefaultChannelCount = 1;
	public static readonly WaveFormat DefaultWaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(DefaultSampleRate, DefaultChannelCount);
}

