extends GraphEdit
class_name Workspace


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
	var sample_count := 512 # Debug value, derive from audio player later
	_update_nodes(sample_count)


func _update_nodes(sample_count: int) -> void:
	# BaeNode -> Array[PackedVector2]
	var output_cache := Dictionary()
	var order := _get_compute_order()
	for node in order:
		# Set all inputs
		_get_nice_connections()\
			.filter(func(c: Connection): c.to == node)\
			.map(func(c: Connection): node.push_input(c.to_port, output_cache[c.from][c.from_port]))
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
 

func _on_disconnection_request(from_node: StringName, from_port: int, to_node: StringName, to_port: int) -> void:
	disconnect_node(from_node, from_port, to_node, to_port)
	var from := get_node(str(from_node)) as BaseNode
	var to := get_node(str(to_node)) as BaseNode


# Get all nodes without consumers
func _get_output_nodes() -> Array[BaseNode]:
	var nodes : Array[BaseNode] = []
	for node in get_tree().get_nodes_in_group("output"):
		nodes.append(node)
	return nodes


# Recompute the global order of computation for all nodes
func _get_compute_order() -> Array[BaseNode]:
	var order : Array[BaseNode] = []
	for output in _get_output_nodes():
		_build_tree(output, order)
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
