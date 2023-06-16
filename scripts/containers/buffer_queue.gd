class_name BufferQueue
extends RefCounted


var _buffers : Array[SynthBuffer]


func size() -> int:
	return _buffers.size()


func enqueue(buffer: SynthBuffer) -> int:
	_buffers.append(buffer)
	return size() - 1


func dequeue(n: int):
	_buffers = _buffers.slice(n)


func at(index: int) -> SynthBuffer:
	return _buffers[index]
