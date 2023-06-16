extends GraphEdit
class_name Workspace


# Emitted when a connection is made, not just requested
signal connection_created(connection: Connection)
# Emitted when a connection is destroyed, not just requested
signal connection_destroyed(connection: Connection)


func _on_connection_request(from_node: StringName, from_port: int, to_node: StringName, to_port: int) -> void:
	var from := get_node(str(from_node)) as SynthNode
	var to := get_node(str(to_node)) as SynthNode
	if _is_port_taken(to, to_port):
		return
		
	connect_node(from_node, from_port, to_node, to_port)
	self.connection_created.emit(Connection.new(from,from_port,to,to_port))
	_rebuild_compute_order()
 

func _on_disconnection_request(from_node: StringName, from_port: int, to_node: StringName, to_port: int) -> void:
	disconnect_node(from_node, from_port, to_node, to_port)
	var from := get_node(str(from_node)) as SynthNode
	var to := get_node(str(to_node)) as SynthNode
	self.connection_destroyed.emit(Connection.new(from,from_port,to,to_port))
	_rebuild_compute_order()


func _is_port_taken(to: SynthNode, port: int) -> bool:
	return _get_inputs(to, port).size() > 0


func _get_inputs(to: SynthNode, port: int) -> Array[SynthNode]:
	var input_nodes : Array[SynthNode] = []
	for node in get_connection_list()\
		.filter(func(c): return str(c.to) == to.name)\
		.filter(func(c): return c.to_port == port)\
		.map(func(c): return get_node(str(c.from))):
		input_nodes.append(node as SynthNode)
	return input_nodes as Array[SynthNode]


# Convert the awful get_connection_list() output to slightly better output
func _get_nice_connections() -> Array[Connection]:
	var connections: Array[Connection] = []
	for c in get_connection_list():
		var connection := Connection.new(
			get_node(str(c.from)) as SynthNode,
			c.from_port,
			get_node(str(c.to)) as SynthNode,
			c.to_port
		)
		connections.append(connection)
	return connections


# Get order of computation for node graph
func dfs(node: SynthNode, order: Array[SynthNode]) -> void:
	if node in order:
		return
	print("descend into %s" % node.name)
	order.append(node)
	var input_nodes := _get_nice_connections().filter(
		func(c: Connection) -> bool:
			return c.to == node
	).map(
		func(c: Connection) -> SynthNode:
			return c.to
	)
	for input_node in input_nodes:
		dfs(input_node, order)


func _on_popup_request(position: Vector2) -> void:
	$AddMenu.position = position
	$AddMenu.show()


func _on_add_menu_add_node(node: SynthNode) -> void:
	add_child(node)
	_rebuild_compute_order()


func _rebuild_compute_order() -> void:
	var order : Array[SynthNode] = []
	var output_nodes := get_tree().get_nodes_in_group("output_node")
	for node in output_nodes:
		dfs(node, order)
	print(order)
