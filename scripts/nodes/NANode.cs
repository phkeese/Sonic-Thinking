using Godot;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SonicThinking.scripts.nodes;

public enum SignalTypes : int
{
	/// <summary>
	/// Audio signal.
	/// </summary>
	Wave = 1,
	
	/// <summary>
	/// Binary enable signal.
	/// </summary>
	Gate = 2,
	
	/// <summary>
	/// Frequency signal in 1V/Octave
	/// </summary>
	Tone = 3,
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
		var slot = GetConnectionInputSlot(toPort);
		var input= from.GetOutput(fromPort);
		InputChanged?.Invoke(this, slot, input);
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
	public delegate void InputChangedHandler(NANode sender, int slotIndex, ISampleProvider input);

	/// <summary>
	/// Fired when an input connection is made or broken.
	/// </summary>
	public InputChangedHandler InputChanged;

	public static readonly WaveFormat DefaultWaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(48000, 2);
	public static readonly int DefaultSampleRate = 48_000;
	public static readonly int DefaultChannelCount = 2;
}

