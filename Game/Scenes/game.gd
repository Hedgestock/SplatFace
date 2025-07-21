extends Node

class_name Game

@export var levels: Array[PackedScene]
@export var levels_container: Node

func _ready():
	ScoreSourceOfTruth.emit_signal("score_changed")
	add_level()

var current_level_number: int = 0
var loaded_levels: Array[Level] = []
func add_level():
	var current_level: Level = levels.pick_random().instantiate()

	current_level.trigger_area.body_entered.connect(on_level_entered)
	current_level.trigger_area.body_exited.connect(on_level_exited)

	current_level.position = Vector2(0, -1024 - (512 * current_level_number))

	loaded_levels.append(current_level)
	levels_container.call_deferred("add_child", current_level)

	current_level_number += 1


func on_level_entered(body: Node2D):
	if (body is Player):
		var current_level = loaded_levels.back()
		current_level.trigger_area.body_entered.disconnect(on_level_entered)

		add_level()

func on_level_exited(body: Node2D):
	if (body is Player && body.GlobalPosition.Y > -256 - (512 * current_level_number)):
		var to_unload: Level = loaded_levels.pop_back()
		levels_container.call_deferred("remove_child", to_unload)

		var current_level: Level = loaded_levels[0] #LoadedLevels.Peek();

		current_level.trigger_area.body_entered.connect(on_level_entered)
		current_level_number -= 1
