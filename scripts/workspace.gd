extends GraphEdit


enum SignalTypes {
	Gate = 0,
	Frequency = 1,
	Tone = 2,
	Waveform = 3,
}


func _process(delta: float) -> void:
	pass


func _on_connection_request(from_node: StringName, from_port: int, to_node: StringName, to_port: int) -> void:
	connect_node(from_node, from_port, to_node, to_port)
	var from := get_node(str(from_node)) as BaseNode
	var to := get_node(str(to_node)) as BaseNode
	to.add_input(from, from_port, to_port)
 

func _on_disconnection_request(from_node: StringName, from_port: int, to_node: StringName, to_port: int) -> void:
	disconnect_node(from_node, from_port, to_node, to_port)
	var from := get_node(str(from_node)) as BaseNode
	var to := get_node(str(to_node)) as BaseNode
	to.remove_input(from, from_port, to_port)


func _build_graph():
	var c := get_connection_list()

