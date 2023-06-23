using Godot;
using System;
using SonicThinking.scripts.nodes;

public partial class NAWorkspace : GraphEdit
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (var left in Enum.GetValues<SignalTypes>())
		{
			AddValidConnectionType((int)left, (int)SignalTypes.Debug);
		}
		
		
		ConnectionRequest += OnConnectionRequest;
		DisconnectionRequest += OnDisconnectionRequest;

		_nodeMenu = GetNode<PopupMenu>("NodeMenu");
		foreach (var scene in NodeScenes)
		{
			var instance = scene.Instantiate<GraphNode>();
			var name = instance.Title;
			instance.QueueFree();
			_nodeMenu.AddItem(name);
		}

		_nodeMenu.IndexPressed += OnNodeSpawnRequested;

		PopupRequest += position =>
		{
			_nodeMenu.Position = (Vector2I)position;
			_nodeMenu.Popup();
		};
	}

	private void OnNodeSpawnRequested(long index)
	{
		var position = _nodeMenu.Position;
		var instance = NodeScenes[index].Instantiate<NANode>();
		AddChild(instance);
		instance.PositionOffset = position;
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

	private bool IsInputTaken(StringName toName, long toPort)
	{
		foreach (var connection in GetConnectionList())
		{
			var to = connection["to"].AsStringName();
			var port = connection["to_port"].AsInt32();
			if (to == toName && port == toPort) return true;
		}

		return false;
	}

	private void OnConnectionRequest(StringName fromName, long fromPort, StringName toName, long toPort)
	{
		if (IsInputTaken(toName, toPort)) ForceDisconnect(toName, toPort);
		
		NANode from = GetNode<NANode>(fromName.ToString());
		NANode to = GetNode<NANode>(toName.ToString());

		if (!IsNodeConnected(fromName, (int)fromPort, toName, (int)toPort))
		{
			to.ConnectInput(from, (int)fromPort, (int)toPort);
		}

		ConnectNode(fromName, (int)fromPort, toName, (int)toPort);
	}

	private void ForceDisconnect(StringName toName, long toPort)
	{
		foreach (var connection in GetConnectionList())
		{
			var to = connection["to"].AsStringName();
			var port = connection["to_port"].AsInt32();
			if (to == toName && port == toPort)
			{
				var fromName = connection["from"].AsStringName();
				var fromPort = connection["from_port"].AsInt32();
				OnDisconnectionRequest(fromName, fromPort, toName, toPort);
			}
		}
	}

	private PopupMenu _nodeMenu;
	
	[Export] public PackedScene[] NodeScenes = new PackedScene[]{};
}
