extends Node

signal score_changed

var score: float:
	get:
		return score
	set(value):
		if (score >= value):
			return
		score = value
		emit_signal("score_changed")
