extends Label


@export var data : Dictionary


func _process(delta):
	var new_text := ""
	for key in data.keys():
		new_text += "%s: %s\n" % [key, data[key]]
	text = new_text
