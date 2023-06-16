extends BaseNode


func _on_add_pressed() -> void:
	var type_index := $AddMenu/Type.get_selected_id() as int
	var new_text := $AddMenu/Type.get_item_text(type_index) as String
	var label := Label.new()
	label.text = new_text
	self.add_child(label)
	self.move_child(label, -2)
	set_slot(get_child_count() - 3, true, type_index, Color.WHITE, true, type_index, Color.WHITE)
