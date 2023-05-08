extends PopupMenu

signal add_node(node: BaseNode)


@export var node_scenes : Array[PackedScene]


func _ready() -> void:
	for scene in node_scenes:
		var instance := scene.instantiate() as BaseNode
		add_item(instance.title)
		instance.queue_free()


func _on_index_pressed(index: int) -> void:
	var scene := node_scenes[index]
	var instance := scene.instantiate() as BaseNode
	instance.position_offset = position
	add_node.emit(instance)
