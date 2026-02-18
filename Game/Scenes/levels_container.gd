extends Node

class_name LevelsContainer

@export var levels: Array[PackedScene]
@export var player: Player

func _ready():
	ScoreSourceOfTruth.emit_signal("score_changed")

var current_level_number: int = 0
func add_level(height):
	if (height < player.global_position.y):
		print("loaded level at height " + str(height))
		var current_level: Level = levels.pick_random().instantiate()
		current_level.position = Vector2(0, -1024 - (512 * current_level_number))

		call_deferred("add_child", current_level)
		current_level_number += 1

func on_screen_exited(level: Level):
	if (level.global_position.y < player.global_position.y):
		print("unloaded level " + level.debug_name)
		call_deferred("remove_child", level)
		current_level_number -= 1


func _on_load_trigger_screen_entered():
	add_level(-1024)
