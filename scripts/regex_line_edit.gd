@tool
extends LineEdit


@export var pattern := ".*" : set = _set_pattern
var _regex : RegEx = RegEx.create_from_string(".*")


func _ready() -> void:
	connect("text_submitted", self._on_text_submitted)


func _set_pattern(value: String) -> void:
	pattern = value
	var new_re := RegEx.create_from_string(value)
	if new_re.is_valid():
		_regex = new_re


func _on_text_submitted(value: String) -> void:
	pass


func _matches(value: String) -> bool:
	var re_match := _regex.search(value)
	return re_match and re_match.get_string() == value
