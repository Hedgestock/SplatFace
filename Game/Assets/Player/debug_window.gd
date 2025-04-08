extends Window

func _ready():
	get_viewport().world_2d = get_tree().root.world_2d;
