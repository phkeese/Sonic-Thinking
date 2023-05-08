extends GraphEdit
class_name Workspace


enum SignalTypes {
	Gate = 0,
	Frequency = 1,
	Tone = 2,
	Waveform = 3,
}


@onready var _output_node := $Output


func _process(delta: float) -> void:
	var sample_count := 512 # Debug value, derive from audio player later


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
	


# Compute order of execution for a specific node
func _build_compute_order(to: BaseNode, order: Array[BaseNode] = []) -> Array[BaseNode]:
	# { from_port: 0, from: "GraphNode name 0", to_port: 1, to: "GraphNode name 1" }.
	var input_nodes := get_connection_list()\
		.filter(func(c): return str(c.to) == to.name)\
		.map(func(c): return get_node(str(c.from)))\
		.filter(func(node): return node not in order)
	for input in input_nodes:
		_build_compute_order(input as BaseNode, order)
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
