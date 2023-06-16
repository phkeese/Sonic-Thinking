class_name SynthNode
extends GraphNode


# Queues this node samples and fills.
# Children must resize these according to their configuration on read()
var inputs : Array[BufferQueue]
var outputs : Array[BufferQueue]
