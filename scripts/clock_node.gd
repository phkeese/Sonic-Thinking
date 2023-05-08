@tool
extends BaseNode


var _period : float = 1.0


func run(sample_count: int):
	var samples_per_period := _period * SynthGlobals.sample_rate
	var on_time := samples_per_period / 2
	var output := PackedVector2Array()
	for i in sample_count:
		var now := SynthGlobals.sample_index + i
		var in_period := fmod(now, samples_per_period)
		var is_on := in_period < on_time
		if is_on:
			output.push_back(Vector2.ONE)
		else:
			output.push_back(Vector2.ZERO)
	push_output(0, output)


func update(delta: float) -> void:
	if not Engine.is_editor_hint():
		$Display.modulate *= 0.99


func _on_bpm_changed(value: String) -> void:
	if value.is_valid_float():
		var bpm := float(value)
		_period = 60 / bpm
