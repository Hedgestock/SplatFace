extends Label

func _ready():
	ScoreSourceOfTruth.score_changed.connect(on_score_changed)

func on_score_changed():
	text = str("%0.2f" % ScoreSourceOfTruth.score," m")
