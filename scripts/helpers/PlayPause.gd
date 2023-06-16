extends Button


const PLAY_ICON := preload("res://assets/ui/arrowRight.png")
const PAUSE_ICON := preload("res://assets/ui/pause.png")


func _ready():
	_on_toggled(button_pressed)


func _on_toggled(button_pressed: bool):
	if button_pressed:
		icon = PAUSE_ICON
		text = "Pause"
	else:
		icon = PLAY_ICON
		text = "Play"
