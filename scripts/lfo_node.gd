extends BaseNode


func _ready() -> void:
	super._ready()
	self.connection_created.connect(self._on_connection)
	self.connection_destroyed.connect(self._on_disconnection)


func run(sample_count: int) -> void:
	pass


func _on_connection(connection: Connection) -> void:
	if connection.is_consumer(self):
		$Debug.modulate = Color.RED


func _on_disconnection(connection: Connection) -> void:
	if connection.is_consumer(self):
		$Debug.modulate = Color.GREEN
