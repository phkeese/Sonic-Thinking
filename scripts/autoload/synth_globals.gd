extends Node


signal started_playing()
signal stopped_playing()


var sample_rate := 48_000 :
	set(value):
		if not is_playing():
			sample_rate = value


var _is_playing := false


var sample_index := 0 :
	get:
		return sample_index


var buffer_size := 1024 :
	set(value):
		if not is_playing():
			buffer_size = value


func is_playing() -> bool:
	return _is_playing


func play() -> void:
	_is_playing = true
	self.started_playing.emit()


func pause() -> void:
	_is_playing = false
	self.stopped_playing.emit()


func rewind() -> void:
	pause()
	sample_index = 0


func advance(sample_count: int) -> void:
	sample_index += sample_count
