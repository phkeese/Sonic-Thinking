extends Panel


@onready var _save_dialog := $SaveDialog
@onready var _open_dialog := $OpenDialog


func _on_file_id_pressed(id: int):
	match id:
		0: _open_dialog.popup()
		1: _save_dialog.popup()


func _on_save_dialog_file_selected(path: String):
	print_debug("Saving to %s" % path)
	var state = $VBoxContainer/NAWorkspace.Serialize()
	print_debug(state)
	var file = FileAccess.open(path, FileAccess.WRITE)
	file.store_var(state)
	file.close()



func _on_open_dialog_file_selected(path: String):
	print_debug("Loading %s" % path)
	var file = FileAccess.open(path, FileAccess.READ)
	var content = file.get_var(true)
	print_debug("read content %s" % content)
	$VBoxContainer/NAWorkspace.Deserialize(content)

