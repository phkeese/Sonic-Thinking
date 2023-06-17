@tool
extends Label


@export var trackers : Array[String]


func _process(delta):
	var string := ""
	for key in trackers:
		string += "%s: %s\n" % [key, get_parent().get(key)]
	text = string
