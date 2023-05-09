extends RefCounted
class_name Connection


var from: BaseNode
var to: BaseNode
var from_port: int
var to_port: int
	

func _init(from: BaseNode, from_port: int, to: BaseNode, to_port: int) -> void:
	self.from = from
	self.from_port = from_port
	self.to = to
	self.to_port = to_port


func is_subject(node: BaseNode) -> bool:
	return is_consumer(node) or is_producer(node)

func is_producer(node: BaseNode) -> bool:
	return node == from

func is_consumer(node: BaseNode) -> bool:
	return node == to
