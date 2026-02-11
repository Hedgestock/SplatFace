extends Node2D

class_name Level

@export var load_trigger = VisibleOnScreenNotifier2D
@export var unload_trigger = VisibleOnScreenNotifier2D

func _on_unload_trigger_screen_exited():
	print("unloading")
	get_parent().on_screen_exited(self)


func _on_load_trigger_screen_entered():
	print("loading")
	get_parent().add_level(load_trigger.global_position.y)
