using Godot;
using System;
using SonicThinking.scripts.nodes;

public partial class NAWorkspace : GraphEdit
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddValidConnectionType((int)SignalTypes.Wave, (int)SignalTypes.Wave);
		
		ConnectionRequest += OnConnectionRequest;
		DisconnectionRequest += OnDisconnectionRequest;
	}

	private void OnDisconnectionRequest(StringName fromName, long fromPort, StringName toName, long toPort)
	{
		NANode from = GetNode<NANode>(fromName.ToString());
		NANode to = GetNode<NANode>(toName.ToString());

		if (IsNodeConnected(fromName, (int)fromPort, toName, (int)toPort))
		{
			to.DisconnectInput(from, (int)fromPort, (int)toPort);
		}
		
		DisconnectNode(fromName, (int)fromPort, toName, (int)toPort);
	}

	private void OnConnectionRequest(StringName fromName, long fromPort, StringName toName, long toPort)
	{
		NANode from = GetNode<NANode>(fromName.ToString());
		NANode to = GetNode<NANode>(toName.ToString());

		if (!IsNodeConnected(fromName, (int)fromPort, toName, (int)toPort))
		{
			to.ConnectInput(from, (int)fromPort, (int)toPort);
		}

		ConnectNode(fromName, (int)fromPort, toName, (int)toPort);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
