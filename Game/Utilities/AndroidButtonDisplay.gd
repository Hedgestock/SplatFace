extends Node2D

func _ready():
	visible = OS.has_feature("mobile")
