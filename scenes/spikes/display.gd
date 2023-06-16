@tool
extends Panel


@export var values : Array[float]


func _draw():
	if values.size() == 0:
		return
		
	if values.size() == 1:
		draw_line(left_center(), right_center(), Color.RED)
	
	for i in values.size() - 1:
		var here := values[i]
		var next := values[i + 1]


func left_center() -> Vector2:
	return Vector2(0, get_rect().size.y / 2)


func right_center() -> Vector2:
	return Vector2(get_rect().size.x, get_rect().size.y / 2)
