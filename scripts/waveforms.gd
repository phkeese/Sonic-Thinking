@tool
extends Panel


@export var data : PackedVector2Array = [Vector2.ZERO,Vector2.ONE,Vector2.ZERO,-Vector2.ONE,Vector2.ZERO] :
	set(value):
		data = value
		self.queue_redraw()


func _draw() -> void:
	if data.is_empty():
		return
	var x_step := get_rect().size.x / (data.size() - 1)
	var y_scale := get_rect().size.y / 2
	var last := -data[0] * y_scale + Vector2.ONE * y_scale
	for i in range(1,data.size()):
		var here := -data[i] * y_scale + Vector2.ONE * y_scale
		var last_x := (i - 1) * x_step
		var x := i * x_step
		draw_line(Vector2(last_x,last.x),Vector2(x,here.x), Color.RED)
		draw_line(Vector2(last_x,last.y),Vector2(x,here.y), Color.GREEN)
		last = here
