extends BaseNode


var _sample_index := 0
var _samples := PackedVector2Array()


func run(sample_count: int):
	_samples = consume_input(0, sample_count)
	var avg := Vector2.ZERO
	for s in _samples:
		avg += abs(s)
	avg /= _samples.size()
	_sample_index = 0
	
	if avg.x > 0:
		$LED.turn_on()
	else:
		$LED.turn_off()
	if not $AudioStreamPlayer.playing and avg.x > 0:
		$AudioStreamPlayer.play()
	
	$Levels/Left.value = avg.x
	$Levels/Right.value = avg.y
