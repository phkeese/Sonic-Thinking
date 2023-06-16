extends RefCounted
class_name Connection


var from: SynthNode
var to: SynthNode
var from_port: int
var to_port: int
	

func _init(from: SynthNode, from_port: int, to: SynthNode, to_port: int) -> void:
	self.from = from
	self.from_port = from_port
	self.to = to
	self.to_port = to_port


func is_subject(node: SynthNode) -> bool:
	return is_consumer(node) or is_producer(node)

func is_producer(node: SynthNode) -> bool:
	return node == from

func is_consumer(node: SynthNode) -> bool:
	return node == to
