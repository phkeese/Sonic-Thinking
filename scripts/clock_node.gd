@tool
extends BaseNode


var _trigger := false


func get_value(slot: int) -> bool:
	return _trigger


func update(delta: float) -> void:
	if not Engine.is_editor_hint():
		$Display.modulate *= 0.99


func _on_bpm_changed(value: String) -> void:
	if value.is_valid_float():
		var bpm := float(value)
		var period := 60 / bpm
		$Timer.wait_time = period
		$Timer.start()


func _on_timer_timeout() -> void:
	_trigger = !_trigger
	if _trigger:
		$Display.modulate = Color.WHITE

