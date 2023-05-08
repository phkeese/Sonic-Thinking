extends BaseNode


func run(sample_count: int):
	var input := consume_input(0, sample_count)
	if input[0].x > 0:
		$LED.turn_on()
	else:
		$LED.turn_off()
