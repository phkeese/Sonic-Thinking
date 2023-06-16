extends PanelContainer


@export_range(0.0,20_000.0, 0.1, "suffix:hz") var frequency := 440.0


@onready var _player := %Player
@onready var _stream : AudioStreamGenerator = _player.stream
@onready var _playback : AudioStreamGeneratorPlayback

@onready var _display := %Display


# Time in seconds since start of playing
var _phi := 0.0

# Toggle playing on or off
var _is_playing := false


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if !_is_playing:
		return
	
	var sample_rate := _stream.mix_rate
	var dx_per_sample := 1.0 / sample_rate
	var needed_frames := _playback.get_frames_available()
	var needed_buffers := needed_frames / SynthGlobals.buffer_size
	if needed_buffers == 0:
		return
	
	for buffer_index in needed_buffers:
		var base_index := buffer_index * SynthGlobals.buffer_size
		var f = func(i: int, _d) -> Vector2:
			var x := _phi + (base_index + i) * dx_per_sample
			var y := sin(TAU * frequency * x)
			return Vector2.ONE * y
		var buffer := SynthBuffer.new().indexed_map(f)
		for i in buffer.size():
			_playback.push_frame(buffer.at(i))
		
		var amplitude := buffer.reduce(Vector2.ZERO,
			func(amplitude: Vector2, sample: Vector2) -> Vector2:
				amplitude.x = max(amplitude.x, abs(sample.x))
				amplitude.y = max(amplitude.y, abs(sample.y))
				return amplitude
		) as Vector2
		_display.push(amplitude)
		
	var sample_count := needed_buffers * SynthGlobals.buffer_size
	_phi += sample_count * dx_per_sample
	print("generated %s buffers (%s samples) phi=%s" % [needed_buffers, sample_count,_phi])


func _on_play_pressed():
	if _is_playing:
		return
	
	_is_playing = true
	_player.play()
	_playback = _player.get_stream_playback()
	


func _on_pause_pressed():
	if !_is_playing:
		return
	
	_is_playing = false;
	_player.stop()
	_playback = null



func _set(value, extra_arg_0):
	pass # Replace with function body.


func _on_frequency_value_changed(value: float):
	frequency = value
