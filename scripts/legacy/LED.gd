@tool
extends TextureRect


@export var on_color := Color.RED : 
	set(value):
		on_color = value
		if Engine.is_editor_hint():
			modulate = value

@export var off_color := Color.BLACK :
	set(value):
		off_color = value
		if Engine.is_editor_hint():
			modulate = value


func turn_on() -> void:
	modulate = on_color


func turn_off() -> void:
	modulate = off_color
