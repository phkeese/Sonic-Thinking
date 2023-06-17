class_name BufferQueue
extends RefCounted


var _buffers : Array[SynthBuffer]


func clear() -> void:
	_buffers.clear()


func size() -> int:
	return _buffers.size()


func enqueue(buffer: SynthBuffer) -> int:
	_buffers.append(buffer)
	return size() - 1


func dequeue() -> SynthBuffer:
	var buffer := _buffers[0]
	_buffers = _buffers.slice(1)
	return buffer


func at(index: int) -> SynthBuffer:
	return _buffers[index]
