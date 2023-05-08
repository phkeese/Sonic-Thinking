extends BaseNode


func _process(delta: float) -> void:
	$HBoxContainer/LineEdit.text = str($Slider.value)


func run(sample_count: int):
	var data := PackedVector2Array()
	data.resize(sample_count)
	data.fill(Vector2.ONE * $Slider.value)
	push_output(0, data)

