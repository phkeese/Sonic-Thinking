using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot.Collections;
using SonicThinking.scripts.nodes;

public partial class NAWorkspace : GraphEdit
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (var left in Enum.GetValues<SignalType>())
		{
			AddValidConnectionType((int)left, (int)SignalType.Any);
			AddValidConnectionType((int)SignalType.Any, (int)left);
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
			var screenPos = GetScreenPosition() + position;
			_nodeMenu.Position = (Vector2I)screenPos;
			_nodeMenu.Popup();
		};
	}

	/// <summary>
	/// Clear all connections and audio nodes.
	/// </summary>
	public void Clear()
	{
		// Clear connections
		foreach (var connection in GetConnectionList())
		{
			EmitSignal(SignalName.DisconnectionRequest, connection["from"], connection["from_port"], connection["to"],
				connection["to_port"]);
		}
		
		// Clear nodes
		foreach (var node in _nodes)
		{
			RemoveChild(node);
			node.QueueFree();
		}
		_nodes.Clear();
	}

	public Dictionary Serialize()
	{
		var nodes = new Array<Dictionary>();
		foreach (var node in _nodes)
		{
			var naNode = node as NANode;
			Debug.Assert(naNode != null, nameof(naNode) + " != null");
			
			var internalState = naNode.Serialize();
			var nodePosition = naNode.PositionOffset;
			var state = new Dictionary()
			{
				{ "position", nodePosition },
				{ "name", node.Name},
				{ "scene_path", node.SceneFilePath}
			};
			if (internalState != null)
			{
				state["state"] = (Variant)internalState;
			}
			nodes.Add(state);
		}

		return new Dictionary()
		{
			{ "nodes", nodes },
			{ "connections", GetConnectionList() }
		};
	}

	public void Deserialize(Dictionary state)
	{
		var nodes = state["nodes"].AsGodotArray<Dictionary>();
		foreach (var nodeState in nodes)
		{
			var position = nodeState["position"].AsVector2();
			var name = nodeState["name"].AsString();
			var scenePath = nodeState["scene_path"].AsString();

			var scene = GD.Load<PackedScene>(scenePath);
			var node = scene.Instantiate<NANode>();
			node.PositionOffset = position;
			node.Name = name;
			AddChild(node);
			_nodes.Add(node);
			
			if (nodeState.TryGetValue("state", out var internalState))
			{
				node.Deserialize(internalState.AsGodotDictionary());
			}
		}

		var connections = state["connections"].AsGodotArray<Dictionary>();
		foreach (var connection in connections)
		{
			EmitSignal(SignalName.ConnectionRequest, connection["from"], connection["from_port"], connection["to"],
				connection["to_port"]);
		}
	}

	
	private void OnNodeSpawnRequested(long index)
	{
		var position = _nodeMenu.Position - GetScreenPosition();
		var instance = NodeScenes[index].Instantiate<NANode>();
		AddChild(instance);
		instance.PositionOffset = position;
		_nodes.Add(instance);
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
	private List<NANode> _nodes = new List<NANode>();

	[Export] public PackedScene[] NodeScenes = new PackedScene[]{};
}
