extends Panel


@onready var _sample_rate_input := $VBoxContainer/PlaybackControls/SampleRate


func _ready() -> void:
	SynthGlobals.started_playing.connect(func(): _sample_rate_input.editable = false)
	SynthGlobals.stopped_playing.connect(func(): _sample_rate_input.editable = true)
	SynthGlobals.sample_rate = int(_sample_rate_input.text)


func _on_play_pause_toggled(button_pressed: bool) -> void:
	if button_pressed:
		SynthGlobals.play()
	else:
		SynthGlobals.pause()


func _on_sample_rate_valid_text_submitted(value: String) -> void:
	SynthGlobals.sample_rate = int(value)
