extends BaseNode


var _sample_index := 0
var _samples := PackedVector2Array()


func _ready() -> void:
	super._ready()
	_samples = filled_with(Vector2.ZERO, 512)


func run(sample_count: int):
	var input := consume_input(0, sample_count)
	var l := min(input.size(), _samples.size()) as int
	var appended := _samples.slice(l) + input
	_samples = appended
	
	var avg := Vector2.ZERO
	for s in _samples:
		avg += abs(s)
	avg /= _samples.size()
	_sample_index = 0
	
	if avg.x > 0:
		$LED.turn_on()
	else:
		$LED.turn_off()
	if not $AudioStreamPlayer.playing and avg.x > 0:
		$AudioStreamPlayer.play()
	
	$Levels/Left.value = avg.x
	$Levels/Right.value = avg.y
	$Waveforms.data = _compute_buckets(_samples)


func _compute_buckets(data: PackedVector2Array) -> PackedVector2Array:
	var buckets := PackedVector2Array()
	var count := $Waveforms.get_rect().size.x as float
	var bucket_size := data.size() / count
	buckets.resize(count)
	for i in count:
		var avg := Vector2.ZERO
		for j in bucket_size:
			avg += data[i + j]
		avg /= bucket_size
		buckets[i] = avg
	return buckets
