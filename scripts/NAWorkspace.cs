using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot.Collections;
using SonicThinking.scripts.nodes;
using Array = Godot.Collections.Array;

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
		
		
		ConnectionRequest += ConnectNodes;
		DisconnectionRequest += DisconnectNodes;
		DeleteNodesRequest += DeleteNodes;

		_nodeMenu = GetNode<PopupMenu>("NodeMenu");
		foreach (var scene in NodeScenes)
		{
			var instance = scene.Instantiate<GraphNode>();
			var name = instance.Title;
			instance.QueueFree();
			_nodeMenu.AddItem(name);
		}

		_nodeMenu.IndexPressed += SpawnNode;

		PopupRequest += position =>
		{
			_spawnPosition = (GetViewport().GetMousePosition() + ScrollOffset) / Zoom;;
			var screenPos = GetScreenPosition() + position;
			_nodeMenu.Position = (Vector2I)screenPos;
			_nodeMenu.Popup();
		};
	}

	private void DeleteNodes(Array nodes)
	{
		foreach (var nodeName in nodes)
		{
			var naNode = GetNode<NANode>(nodeName.AsString());
			DeleteNode(naNode);
		}
	}

	private void DeleteNode(NANode node)
	{
		Isolate(node);
		RemoveChild(node);
		_nodes.Remove(node);
		node.QueueFree();
	}

	private void Isolate(NANode naNode)
	{
		foreach (var connection in GetConnectionList())
		{
			// Only consider connections of this node.
			if (!(connection["from"].AsString().Equals(naNode.Name)) && !(connection["to"].AsString().Equals(naNode.Name)))
			{
				continue;
			}
			
			DisconnectNodes(connection["from"].AsStringName(), connection["from_port"].AsInt32(), connection["to"].AsStringName(),connection["to_port"].AsInt32());
		}
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
			var guid = naNode.Guid;
			
			var state = new Dictionary()
			{
				{ "position", nodePosition },
				{ "name", node.Name},
				{ "scene_path", node.SceneFilePath},
				{"guid", guid.ToString()},
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
			{ "connections", GetStableConnections() }
		};
	}

	private Array GetStableConnections()
	{
		var connections = new Array();
		foreach (var connection in GetConnectionList())
		{
			var from = GetNode<NANode>(connection["from"].AsString());
			var to = GetNode<NANode>(connection["to"].AsString());

			var dict = new Dictionary()
			{
				{ "from", from.Guid.ToString() },
				{ "to", to.Guid.ToString() },
				{ "from_port", connection["from_port"] },
				{ "to_port", connection["to_port"] },
			};
			connections.Add(dict);
		}

		return connections;
	}

	private NANode GetStableNode(Guid guid)
	{
		foreach (var node in GetChildren())
		{
			if (node is NANode naNode && naNode.Guid == guid)
			{
				return naNode;
			}
		}

		return null;
	}

	public void Deserialize(Dictionary state)
	{
		var nodes = state["nodes"].AsGodotArray<Dictionary>();
		foreach (var nodeState in nodes)
		{
			var position = nodeState["position"].AsVector2();
			var name = nodeState["name"].AsString();
			var scenePath = nodeState["scene_path"].AsString();
			var guid = Guid.Parse(nodeState["guid"].AsString());

			var scene = GD.Load<PackedScene>(scenePath);
			var node = scene.Instantiate<NANode>();
			node.PositionOffset = position;
			node.Name = name;
			node.Guid = guid;

			// Need to add to tree before deserializing node to give it a chance to find its own child nodes.
			Add(node);
			
			if (nodeState.TryGetValue("state", out var internalState))
			{
				node.Deserialize(internalState.AsGodotDictionary());
			}
		}

		var connections = state["connections"].AsGodotArray<Dictionary>();
		foreach (var connection in connections)
		{
			var from = GetStableNode(Guid.Parse(connection["from"].AsString()));
			var to = GetStableNode(Guid.Parse(connection["to"].AsString()));

			EmitSignal(SignalName.ConnectionRequest, from.Name, connection["from_port"], to.Name,
				connection["to_port"]);
		}
	}

	private void Add(NANode node)
	{
		AddChild(node);
		_nodes.Add(node);
		node.CloseRequest += () => DeleteNode(node);
		node.ResizeRequest += minsize => ResizeNode(node, minsize);
	}

	private void ResizeNode(NANode node, Vector2 minSize)
	{
		node.Size = minSize;
	}


	private void SpawnNode(long index)
	{
		var instance = NodeScenes[index].Instantiate<NANode>();
		instance.PositionOffset = _spawnPosition;

		Add(instance);
	}


	private void DisconnectNodes(StringName fromName, long fromPort, StringName toName, long toPort)
	{
		NANode from = GetNode<NANode>(fromName.ToString());
		NANode to = GetNode<NANode>(toName.ToString());
		
		if (IsNodeConnected(fromName, (int)fromPort, toName, (int)toPort))
		{
			to.DisconnectInput(from, (int)fromPort, (int)toPort);
		}
		
		base.DisconnectNode(fromName, (int)fromPort, toName, (int)toPort);
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

	private void ConnectNodes(StringName fromName, long fromPort, StringName toName, long toPort)
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
				DisconnectNodes(fromName, fromPort, toName, toPort);
			}
		}
	}
	
	[Export] public PackedScene[] NodeScenes = new PackedScene[]{};

	private PopupMenu _nodeMenu;
	private readonly List<NANode> _nodes = new List<NANode>();
	private Vector2 _spawnPosition = new Vector2();

}
