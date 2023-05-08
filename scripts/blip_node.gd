extends BaseNode


var _clock_state := false


func update(delta: float) -> void:
	if _should_trigger():
		$Player.play()


func _should_trigger() -> bool:
	var inputs := _get_inputs(0)
	if inputs.is_empty():
		return false
	var clock := inputs[0].from_node
	var value := clock.get_value(0) as bool
	var old_state := _clock_state
	_clock_state = value
	return value and not old_state
