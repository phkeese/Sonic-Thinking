extends SynthNode


@onready var _player := $Player as AudioStreamPlayer
@onready var _generator := _player.stream as AudioStreamGenerator
@onready var _playback := _generator.instantiate_playback() as AudioStreamGeneratorPlayback


var _output_queue := BufferQueue.new()


func _ready():
	inputs.resize(1)
	output = BufferQueue.new()
	_generator.mix_rate = SynthGlobals.sample_rate
	_player.play()
	


func _process(delta):
	var needed_frames := _playback.get_frames_available()
	var needed_buffers = needed_frames / SynthGlobals.buffer_size
	var buffers_available = min(needed_buffers, _output_queue.size())
	for i in buffers_available:
		var buffer := _output_queue.dequeue()
		_playback.push_buffer(PackedVector2Array(buffer.data))
	if buffers_available > 0:
		print("pushed %s buffers" % buffers_available)
	


func _next() -> SynthBuffer:
	_output_queue.enqueue(_get_input(0))
	return null
