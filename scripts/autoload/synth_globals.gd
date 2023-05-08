extends Node


var sample_rate := 48_000 :
	get:
		return sample_rate
	set(value):
		if not is_playing:
			sample_rate = value


var is_playing := false :
	get:
		return is_playing


var sample_index := 0 :
	get:
		return sample_index


func play() -> void:
	is_playing = true


func pause() -> void:
	is_playing = false


func rewind() -> void:
	pause()
	sample_index = 0


func advance(sample_count: int) -> void:
	sample_index += sample_count
