extends Node2D

@export var test: CanvasLayer

func _ready():
	visible = DisplayServer.is_touchscreen_available()
	if OS.has_feature("mobile"):
		get_tree().root.content_scale_aspect = Window.CONTENT_SCALE_ASPECT_KEEP_WIDTH
		position = Vector2(0, get_viewport().get_visible_rect().size.y)
