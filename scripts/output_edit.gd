@tool
extends HBoxContainer
class_name OutputEdit

signal changed(value: String)


enum InputType {
	Text,
	Number,
}


@export var label := "Label" : set = _set_label
@export var regex := ".*" : set = _set_regex
@export var placeholder_text := "Value..." : set = _set_placeholder
@export var value : String : set = _set_value


var _regex : RegEx


func _verify_regex(value: String) -> bool:
	if not _regex or not _regex.is_valid():
		return false
	var result = _regex.search(value)
	return result and result.get_string() == value


func _set_label(value: String):
	label = value
	$Label.text = value



func _set_value(new_value: String):
	if new_value.is_empty() or _verify_regex(new_value):
		value = new_value
		$LineEdit.set("text", new_value)
		emit_signal("changed", value)


func _set_placeholder(new_text: String):
	placeholder_text = new_text
	$LineEdit.placeholder_text = new_text


func _set_regex(new_value: String):
	regex = new_value
	_regex = RegEx.create_from_string(new_value)
