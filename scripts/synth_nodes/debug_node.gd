extends SynthNode


@onready var _wave := %Wave

# Called when the node enters the scene tree for the first time.
func _ready():
	inputs.resize(1)
	output = BufferQueue.new()


func _next() -> SynthBuffer:
	var buffer := _get_input(0)
	var wave_avg := buffer.average()
	_wave.push(wave_avg)
	_wave.data = PackedVector2Array(buffer.data)
	return null
