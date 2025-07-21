extends Node

signal score_changed

var _score_path = "user://score"
var _score_file

var score: float:
	get:
		return score
	set(value):
		if (score >= value):
			return
		score = value
		emit_signal("score_changed")
		_score_file.set_value("", "score", value)
		_score_file.save(_score_path)

func _ready():
	_score_file = ConfigFile.new()
	var err = _score_file.load(_score_path)
	score = _score_file.get_value("", "score", 0)
