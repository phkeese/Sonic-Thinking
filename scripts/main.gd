extends Panel


func _on_play_pause_toggled(button_pressed: bool) -> void:
	if button_pressed:
		SynthGlobals.play()
	else:
		SynthGlobals.pause()
