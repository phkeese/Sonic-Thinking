extends SynthNode


const FRQ_INPUT = 0
const EN_INPUT = 2

# Current angle
@export var phi := 0.0

# Current buffer index
@export var current_buffer_index := 0

# Frequency of tone
@export var frequency := 440.0
@onready var _frequency_input := %Frequency


# Called when the node enters the scene tree for the first time.
func _ready():
	inputs.resize(3)


func _next() -> SynthBuffer:
	var dx := 1.0 / SynthGlobals.sample_rate
	var frequency := _get_frequency(current_buffer_index)
	current_buffer_index += 1
	var buffer := frequency.indexed_map(
		func(i: int, frq: Vector2) -> Vector2:
			var x := phi + dx * i;
			var y := sin(TAU * frq.x * x)
			return Vector2.ONE * y
	)
	return buffer


func _get_frequency(buffer_index: int) -> SynthBuffer:
	if inputs[FRQ_INPUT] == null:
		return SynthBuffer.with_value(Vector2.ONE * _frequency_input.value)
	return inputs[FRQ_INPUT].at(buffer_index)
