extends BaseNode


var _frequency := 0.0
var _use_own_input := true


func _ready() -> void:
	super._ready()
	self.connection_created.connect(self._on_connection)
	self.connection_destroyed.connect(self._on_disconnection)


func run(sample_count: int) -> void:
	var input : PackedVector2Array
	if _use_own_input:
		input = pad_with(input, Vector2.ONE * _frequency, sample_count)
	else:
		input = consume_input(0, sample_count)
	
	var output := filled_with(Vector2.ZERO, sample_count)
	var time_per_sample := 1.0 / SynthGlobals.sample_rate
	for i in sample_count:
		var time := (SynthGlobals.sample_index + i) * time_per_sample
		var x := fmod(time, 1.0) * TAU
		var y := sin(x)
		output[i] = Vector2.ONE * y
	push_output(0, output)


func _on_connection(connection: Connection) -> void:
	if connection.is_consumer(self):
		$FrequencyInput/Frequency.editable = false
		_use_own_input = false


func _on_disconnection(connection: Connection) -> void:
	if connection.is_consumer(self):
		$FrequencyInput/Frequency.editable = true
		_use_own_input = true
