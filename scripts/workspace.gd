extends GraphEdit
class_name Workspace


# Emitted when a connection is made, not just requested
signal connection_created(from: BaseNode, from_port: int, to: BaseNode, to_port: int)
# Emitted when a connection is destroyed, not just requested
signal connection_destroyed(from: BaseNode, from_port: int, to: BaseNode, to_port: int)


enum SignalTypes {
	Gate = 0,
	Frequency = 1,
	Tone = 2,
	Waveform = 3,
}

class Connection:
	var from: BaseNode
	var to: BaseNode
	var from_port: int
	var to_port: int
	
	func _init(from: BaseNode, from_port: int, to: BaseNode, to_port: int) -> void:
		self.from = from
		self.from_port = from_port
		self.to = to
		self.to_port = to_port


@onready var _output_node := $Output


func _process(delta: float) -> void:
	var sample_count := SynthGlobals.sample_rate # Debug value, derive from audio player later
	if SynthGlobals.is_playing:
		_update_nodes(sample_count)
		SynthGlobals.advance(sample_count)
		print("updated nodes with %s samples" % sample_count)


func _update_nodes(sample_count: int) -> void:
	# BaeNode -> Array[PackedVector2]
	var output_cache := Dictionary()
	var order := _get_compute_order()
	for node in order:
		# Set all inputs
		var connections : Array[Connection] = []
		for c in _get_nice_connections():
			if c.to == node:
				connections.append(c)
				
		for conn in connections:
			var from := conn.from
			var from_port := conn.from_port
			var to_port := conn.to_port
			var data := output_cache[from][from_port] as PackedVector2Array
			node.push_input(to_port, data)
			
		# Compute outputs
		node.run(sample_count)
		
		# Cache outputs
		var output : Array[PackedVector2Array] = []
		for slot in node.get_connection_output_count():
			output.append(node.consume_output(slot, sample_count))
		output_cache[node] = output


func _on_connection_request(from_node: StringName, from_port: int, to_node: StringName, to_port: int) -> void:
	var from := get_node(str(from_node)) as BaseNode
	var to := get_node(str(to_node)) as BaseNode
	if _is_port_taken(to, to_port):
		return
		
	connect_node(from_node, from_port, to_node, to_port)
	self.connection_created.emit(from, from_port, to, to_port)
 

func _on_disconnection_request(from_node: StringName, from_port: int, to_node: StringName, to_port: int) -> void:
	disconnect_node(from_node, from_port, to_node, to_port)
	var from := get_node(str(from_node)) as BaseNode
	var to := get_node(str(to_node)) as BaseNode
	self.connection_destroyed.emit(from, from_port, to, to_port)


# Get all audio nodes
func _get_audio_nodes() -> Array[BaseNode]:
	var nodes : Array[BaseNode] = []
	for node in get_tree().get_nodes_in_group("audio"):
		nodes.append(node)
	return nodes


# Recompute the global order of computation for all nodes
func _get_compute_order() -> Array[BaseNode]:
	var order : Array[BaseNode] = []
	for node in _get_audio_nodes():
		_build_tree(node, order)
	return order


# Compute order of execution for a specific node
func _build_tree(to: BaseNode, order: Array[BaseNode] = []) -> Array[BaseNode]:
	# { from_port: 0, from: "GraphNode name 0", to_port: 1, to: "GraphNode name 1" }.
	var input_nodes := get_connection_list()\
		.filter(func(c): return str(c.to) == to.name)\
		.map(func(c): return get_node(str(c.from)))\
		.filter(func(node): return node not in order)
	for input in input_nodes:
		_build_tree(input as BaseNode, order)
	order.append(to)
	return order


func _is_port_taken(to: BaseNode, port: int) -> bool:
	return _get_inputs(to, port).size() > 0


func _get_inputs(to: BaseNode, port: int) -> Array[BaseNode]:
	var input_nodes : Array[BaseNode] = []
	for node in get_connection_list()\
		.filter(func(c): return str(c.to) == to.name)\
		.filter(func(c): return c.to_port == port)\
		.map(func(c): return get_node(str(c.from))):
		input_nodes.append(node as BaseNode)
	return input_nodes as Array[BaseNode]


# Convert the awful get_connection_list() output to slightly better output
func _get_nice_connections() -> Array[Connection]:
	var connections: Array[Connection] = []
	for c in get_connection_list():
		var connection := Connection.new(
			get_node(str(c.from)) as BaseNode,
			c.from_port,
			get_node(str(c.to)) as BaseNode,
			c.to_port
		)
		connections.append(connection)
	return connections


func _on_popup_request(position: Vector2) -> void:
	$AddMenu.position = position
	$AddMenu.show()


func _on_add_menu_add_node(node: BaseNode) -> void:
	add_child(node)
