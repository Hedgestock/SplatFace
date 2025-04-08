extends AudioStreamPlayer2D

@export var running: AudioStream

@export var splatting: AudioStream

@export var landing: AudioStream

func run():
	stream = running
	play()

func splat():
	stream = splatting
	play()

func land():
	stream = landing
	play()
