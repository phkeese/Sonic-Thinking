extends Node


var _queue := []


func queue_action(callable: Callable):
	_queue.append(callable)


func _process(_delta):
	if _queue.is_empty():
		return
	
	var action := _queue.pop_front() as Callable
	action.call()
