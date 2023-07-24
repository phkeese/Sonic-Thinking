extends Panel


@onready var _save_dialog := $SaveDialog
@onready var _open_dialog := $OpenDialog
@onready var _workspace := $VBoxContainer/NAWorkspace


var _filepath : String


func _on_file_id_pressed(id: int):
	match id:
		0: _open_dialog.popup()
		1: _save()
		2: _save_dialog.popup()
		3: 
			_workspace.Clear()
			_filepath = ""
			_save_dialog.current_file = ""


func _save():
	if _filepath == "":
		_save_dialog.show()
		return
	
	print_debug("Saving to %s" % _filepath)
	var state = $VBoxContainer/NAWorkspace.Serialize()
	var file = FileAccess.open(_filepath, FileAccess.WRITE)
	file.store_var(state)
	file.close()



func _open():
	if _filepath == "":
		_open_dialog.show()
		return
	
	print_debug("Loading %s" % _filepath)
	var file = FileAccess.open(_filepath, FileAccess.READ)
	var content = file.get_var(false)
	
	_workspace.Clear()
	_workspace.Deserialize(content)
	

func _on_save_dialog_file_selected(path):
	_filepath = path
	_save()


func _on_open_dialog_file_selected(path):
	_filepath = path
	_open()
