extends Object


func test_size():
	var buffer := SynthBuffer.new()
	assert(buffer.size() == SynthGlobals.buffer_size)


func test_map():
	var f = func(i: int): return Vector2.ONE * i
	var buffer := SynthBuffer.new()
	var counter := buffer.map(f)
	for i in counter.size():
		assert(counter.at(i) == f.call(i))
