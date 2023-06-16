class_name SynthNode
extends GraphNode


# Slot this node uses for its output. 
# MUST BE THE ONLY ENABLED OUTPUT SLOT!
@export var output_slot := 0


# Queues this node samples and fills.
# Children must resize these according to their configuration on read()
var inputs : Array[BufferQueue]
var output : BufferQueue


# Compute and enqueue n buffers of data
func compute(buffer_count: int) -> void:
	for i in buffer_count:
		output.enqueue(_next())


# To be implemented by children
func _next() -> SynthBuffer:
	return SynthBuffer.new()


# Clear output queues before next computation
func dequeue(buffer_count: int) -> void:
	output.dequeue(buffer_count)
