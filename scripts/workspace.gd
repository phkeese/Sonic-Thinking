extends GraphEdit
class_name Workspace


# Emitted when a connection is made, not just requested
signal connection_created(connection: Connection)
# Emitted when a connection is destroyed, not just requested
signal connection_destroyed(connection: Connection)


# Order of node evaluation
var _compute_order : Array[SynthNode]


func _process(_delta):
	if not SynthGlobals.is_playing():
		return
	var needed_buffers := SynthGlobals.sample_rate / 60
	for i in needed_buffers:
		_step()
	
	for node in _compute_order:
		node.finish()


func _step():
	for node in _compute_order:
		node.compute(1)


func _on_connection_request(from_node: StringName, from_port: int, to_node: StringName, to_port: int) -> void:
	var from := get_node(str(from_node)) as SynthNode
	var to := get_node(str(to_node)) as SynthNode
	if _is_port_taken(to, to_port):
		return
	connect_node(from_node, from_port, to_node, to_port)
	
	var f := func(): 
		to.inputs[to_port] = from.output
	f.call_deferred()
	
	_rebuild_compute_order()
	self.connection_created.emit(Connection.new(from,from_port,to,to_port))
 

func _on_disconnection_request(from_node: StringName, from_port: int, to_node: StringName, to_port: int) -> void:
	var from := get_node(str(from_node)) as SynthNode
	var to := get_node(str(to_node)) as SynthNode
	disconnect_node(from_node, from_port, to_node, to_port)
	
	var f := func(): 
		to.inputs[to_port] = null
	f.call_deferred()
	
	_rebuild_compute_order()
	self.connection_destroyed.emit(Connection.new(from,from_port,to,to_port))


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
func dfs(node: SynthNode, order: Array[SynthNode]) -> Array[SynthNode]:
	if node in order:
		return order
	print("descend into %s" % node.name)
	var input_nodes := _get_nice_connections().filter(
		func(c: Connection) -> bool:
			return c.to == node
	).map(
		func(c: Connection) -> SynthNode:
			return c.from
	)
	for input_node in input_nodes:
		dfs(input_node, order)
	order.append(node)
	return order


func _on_popup_request(position: Vector2) -> void:
	var menu := $AddMenu
	if Input.is_key_pressed(KEY_SHIFT):
		menu = $DebugMenu
	menu.position = position
	menu.show()


func _on_add_menu_add_node(node: SynthNode) -> void:
	add_child(node)
	_rebuild_compute_order()


func _rebuild_compute_order() -> void:
	var order : Array[SynthNode] = []
	var synth_nodes := get_tree().get_nodes_in_group("output_node")
	for node in synth_nodes:
		dfs(node, order)
	for node in get_tree().get_nodes_in_group("synth_node"):
		dfs(node, order)
	_compute_order = order
	print(order)


func _on_debug_menu_id_pressed(id: int):
	if id == 0:
		for node in get_children():
			if not node is SynthNode:
				continue
			if not node.get_rect().has_point(get_global_mouse_position()):
				continue
			print("printing inputs of %s" % node)
			var inputs := _get_nice_connections().filter(
				func(c: Connection) -> bool:
					return c.to == node
			).map(
				func(c: Connection) -> SynthNode:
					return c.from
			)
			print(inputs)
