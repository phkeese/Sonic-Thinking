extends GraphNode
class_name BaseNode


var _input_buffers : Dictionary
var _output_buffers : Dictionary


# Push a new buffer to the specified input to be consumed by run()
# Called by Workspace before run()
func push_input(slot: int, data: PackedVector2Array):
	_get_input_buffer(slot).append_array(data)


# Get input from specified slot and pad with Zero
# Called by run() to consume inputs
func consume_input(slot: int, count: int) -> PackedVector2Array:
	var buffer := _get_input_buffer(slot)
	var data := buffer.slice(0,count)
	var leftover := buffer.slice(-count)
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
	_get_output_buffer(slot).append_array(data)


# Get output from specified slot and pad with Zero
# Called by Workspace to consume outputs
func consume_output(slot: int, count: int) -> PackedVector2Array:
	var buffer := _get_output_buffer(slot)
	var data := buffer.slice(0,count)
	var leftover := buffer.slice(-count)
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
	var padding := PackedVector2Array()
	padding.resize(n)
	padding.fill(value)
	data.append_array(padding)
	return data
