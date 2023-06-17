class_name SynthNode
extends GraphNode


# Queues this node samples and fills.
# Children must resize these according to their configuration on read()
var inputs : Array[BufferQueue]
var output : BufferQueue

# Current buffer index
@export var current_buffer_index := 0

# Compute and enqueue n buffers of data
func compute(buffer_count: int) -> void:
	for i in buffer_count:
		output.enqueue(_next())
		current_buffer_index += 1


# To be implemented by children
func _next() -> SynthBuffer:
	return SynthBuffer.new()


# Clear output queues before next computation
func finish():
	output.clear()
	current_buffer_index = 0


# Helper to get inputs.
# If no input is connected, fill with this value.
func _get_input(index: int, default := Vector2.ZERO) -> SynthBuffer:
	var input := inputs[index]
	if input == null:
		print("returned empty buffer to node %s" % name)
		return SynthBuffer.with_value(default)
	return input.at(current_buffer_index)
