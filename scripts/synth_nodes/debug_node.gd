extends SynthNode


@onready var _wave := %Wave

# Called when the node enters the scene tree for the first time.
func _ready():
	inputs.resize(1)


func _next() -> SynthBuffer:
	var wave_avg := _get_input(0).average()
	_wave.push(wave_avg)
	return null
