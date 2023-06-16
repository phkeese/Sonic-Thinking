extends GraphEdit
class_name Workspace


# Emitted when a connection is made, not just requested
signal connection_created(connection: Connection)
# Emitted when a connection is destroyed, not just requested
signal connection_destroyed(connection: Connection)


func _on_connection_request(from_node: StringName, from_port: int, to_node: StringName, to_port: int) -> void:
	var from := get_node(str(from_node)) as BaseNode
	var to := get_node(str(to_node)) as BaseNode
	if _is_port_taken(to, to_port):
		return
		
	connect_node(from_node, from_port, to_node, to_port)
	self.connection_created.emit(Connection.new(from,from_port,to,to_port))
 

func _on_disconnection_request(from_node: StringName, from_port: int, to_node: StringName, to_port: int) -> void:
	disconnect_node(from_node, from_port, to_node, to_port)
	var from := get_node(str(from_node)) as BaseNode
	var to := get_node(str(to_node)) as BaseNode
	self.connection_destroyed.emit(Connection.new(from,from_port,to,to_port))


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
