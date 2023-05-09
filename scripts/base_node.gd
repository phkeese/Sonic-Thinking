extends GraphNode
class_name BaseNode


# Emitted when a connection to this node is made
signal connection_created(connection: Connection)
# Emitted when a connection to this node is destroyed
signal connection_destroyed(connection: Connection)


var _input_buffers : Dictionary
var _output_buffers : Dictionary


func _ready() -> void:
	var parent := get_parent_control() as Workspace
	if not parent:
		return
		
	# Monitor parent for connections and emit own signal in case one is made	
	parent.connection_created.connect(self._on_parent_connection_created)
	parent.connection_destroyed.connect(self._on_parent_connection_destroyed)


# Push a new buffer to the specified input to be consumed by run()
# Called by Workspace before run()
func push_input(slot: int, data: PackedVector2Array):
	var buffer := _get_input_buffer(slot)
	buffer.append_array(data)
	_set_input_buffer(slot,buffer)


# Get input from specified slot and pad with Zero
# Called by run() to consume inputs
func consume_input(slot: int, count: int) -> PackedVector2Array:
	var buffer := _get_input_buffer(slot)
	var data := buffer.slice(0,count)
	var leftover := buffer.slice(count)
	_set_input_buffer(slot, leftover)
	return pad_with(data, Vector2.ZERO, count)


# Compute n new samples and push to outputs
# Should consume n samples from all inputs and optionally push n output samples
func run(sample_count: int):
	# Clears all inputs to avoid memory leaks
	# Replace with your own implementation
	_input_buffers.clear()


# Push a new buffer to the output to be consumed by other nodes
# Called by run() to append to outputs
func push_output(slot: int, data: PackedVector2Array):
	var buffer := _get_output_buffer(slot)
	buffer.append_array(data)
	_output_buffers[slot] = buffer


# Get output from specified slot and pad with Zero
# Called by Workspace to consume outputs
func consume_output(slot: int, count: int) -> PackedVector2Array:
	var buffer := _get_output_buffer(slot)
	var data := buffer.slice(0,count)
	var leftover := buffer.slice(count)
	_set_output_buffer(slot, leftover)
	return pad_with(data, Vector2.ZERO, count)


func _get_input_buffer(slot: int) -> PackedVector2Array:
	return _input_buffers.get(slot, PackedVector2Array()) as PackedVector2Array


func _set_input_buffer(slot: int, data: PackedVector2Array):
	_input_buffers[slot] = data


func _get_output_buffer(slot: int) -> PackedVector2Array:
	return _output_buffers.get(slot, PackedVector2Array()) as PackedVector2Array


func _set_output_buffer(slot: int, data: PackedVector2Array):
	_output_buffers[slot] = data


func pad_with(data: PackedVector2Array, value: Vector2, count: int) -> PackedVector2Array:
	var n := max(count - data.size(), 0) as int
	var padding := filled_with(Vector2.ZERO, n)
	data.append_array(padding)
	return data


func filled_with(value: Vector2, count: int) -> PackedVector2Array:
	var data := PackedVector2Array()
	data.resize(count)
	data.fill(value)
	return data


func _on_parent_connection_created(connection: Connection) -> void:
	if connection.is_subject(self):
		self.connection_created.emit(connection)

func _on_parent_connection_destroyed(connection: Connection) -> void:
	if connection.is_subject(self):
		self.connection_destroyed.emit(connection)
