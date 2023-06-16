class_name SynthBuffer
extends RefCounted


# Data used by this SynthBuffer. Accessed through get() and set()
@export var data : Array[Vector2] :
	set(value):
		pass
	get:
		return data


static func with_value(initial: Vector2) -> SynthBuffer:
	var buffer := SynthBuffer.new()
	buffer.data.fill(initial)
	return buffer


func _init():
	data.resize(SynthGlobals.buffer_size)


# Call f(i,data[i]) for each element of this buffer and return the result in a new
# buffer.
func indexed_map(f: Callable) -> SynthBuffer:
	var new_buffer = SynthBuffer.new()
	for i in size():
		var sample := f.call(i, data[i]) as Vector2
		new_buffer.data[i] = sample
	return new_buffer


# Call f(data[i]) for each element of this buffer and return the result in a new
# buffer.
func map(f: Callable) -> SynthBuffer:
	var new_buffer = SynthBuffer.new()
	for i in size():
		var sample := f.call(data[i]) as Vector2
		new_buffer.data[i] = sample
	return new_buffer


# Call f(data[i]) for each element of this buffer.
func for_each(f: Callable) -> void:
	for i in size():
		f.call(data[i])


# Perform start_value = f(start_value, data[i]) for each element of this buffer
# and return final result.
func reduce(start_value, f: Callable):
	for i in size():
		start_value = f.call(start_value, data[i])
	return start_value


func size() -> int:
	return data.size()


func at(index: int) -> Vector2:
	return data[index]


func average() -> Vector2:
	var sum := reduce(Vector2.ZERO, func(acc, sample): return acc + sample) as Vector2
	return sum / size()

