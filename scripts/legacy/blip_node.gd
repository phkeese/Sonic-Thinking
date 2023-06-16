extends BaseNode


var _clock_state := false


func update(delta: float) -> void:
	if _should_trigger():
		$Player.play()


func _should_trigger() -> bool:
	return false
