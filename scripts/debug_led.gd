extends BaseNode


var _sample_index := 0
var _samples := PackedVector2Array()


func run(sample_count: int):
	_samples = consume_input(0, sample_count)
	print(_samples)
	_sample_index = 0
	if _samples[0].x > 0:
		$LED.turn_on()
	else:
		$LED.turn_off()
	if not $AudioStreamPlayer.playing and _samples[0].x > 0:
		$AudioStreamPlayer.play()
