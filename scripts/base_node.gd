extends GraphNode
class_name BaseNode


class InputConnection:
	var from_node: BaseNode
	var from_slot: int
	var to_slot: int
	
	func _init(from_node: BaseNode, from_slot: int, to_slot: int) -> void:
		self.from_node = from_node
		self.from_slot = from_slot
		self.to_slot = to_slot


var _inputs := {}


func update(delta: float) -> void:
	pass


func add_input(node: BaseNode, from_slot: int, to_slot: int):
	var connection := InputConnection.new(node, from_slot, to_slot)
	
	if connection not in _inputs:
		_inputs[connection] = true


func remove_input(node: BaseNode, from_slot: int, to_slot: int):
	for connection in _inputs.keys():
		if connection.from_node == node and connection.from_slot == from_slot and connection.to_slot == to_slot:
			_inputs.erase(connection)


func get_value(slot: int):
	return null


func _get_inputs(slot: int) -> Array[InputConnection]:
	var connections : Array[InputConnection] = []
	for connection in _inputs.keys():
		if connection.to_slot == slot:
			connections.append(connection)
	return connections


func get_all_input_nodes() -> Array[BaseNode]:
	var input_nodes : Array[BaseNode] = []
	for connection in _inputs.keys():
		input_nodes.append(connection.from_node)
	return input_nodes
