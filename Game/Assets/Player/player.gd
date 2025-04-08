extends RigidBody2D

class_name Player

@export var speed: int = 200
@export var distance_fallen_label: Label
@export var music: AudioStreamPlayer

@export_group("Animation")
@export var sprite: AnimatedSprite2D
@export var animation: AnimationPlayer

@export_group("Physics")
@export var ground_ray: RayCast2D
@export var controlled_physics: PhysicsMaterial
@export var ragdoll_physics: PhysicsMaterial
@export var landing_physics: PhysicsMaterial

@export_group("Debug")
@export var debug_label: Label
@export var debug_window: Window

var _falling_height: float = 0

var distance_fallen: float:
	get:
		return abs(_falling_height - position.y) / 15

var is_actionable: bool:
	get:
		return ground_ray.is_colliding() && !["jump", "fall", "land", "splat", "sit"].has(sprite.animation)

var can_fall: bool:
	get:
		return !["fall", "land", "splat", "sit"].has(sprite.animation)

var _lock_walk_state = false

var is_walking: bool:
	get:
		return Input.is_action_pressed("walk") || sprite.animation == "walk" || _lock_walk_state

var should_ragdoll: bool:
	get:
		return !ground_ray.is_colliding() || sprite.animation == "jump" || (sprite.animation == "fall" && ground_ray.get_collision_normal() != Vector2.UP && distance_fallen > 10)

var current_physics: PhysicsMaterial:
		get:
			if (should_ragdoll):
				return ragdoll_physics
			if (["land", "splat", "sit"].has(sprite.animation) || Input.get_axis("move_left", "move_right") == 0):
				return landing_physics
			return controlled_physics

func _physics_process(delta):
	ground_ray.target_position = Vector2(0, max(linear_velocity.y * 2 * delta, 5))
	if (is_actionable):
		var direction = Input.get_axis("move_left", "move_right")
		if (is_walking):
			direction = clamp(direction, -0.49, 0.49)
		linear_velocity = Vector2(direction * speed, linear_velocity.y if ground_ray.get_collision_normal() != Vector2.UP else 0.0)
		if (animation.assigned_animation != "jump"):
			if (direction == 0):
				animation.play("idle")
			else:
				if (abs(direction) < 0.5 && (animation.assigned_animation != "run")):
					animation.play("walk")
				elif (animation.assigned_animation != "run"):
					animation.play("run")
				sprite.flip_h = direction < 0
	else:
		if (can_fall && linear_velocity.y >= 0):
			fall()
		elif (animation.assigned_animation == "fall" && !should_ragdoll):
			land()
		elif (animation.assigned_animation == "sit" && Input.get_action_strength("sit") != 1):
			idle()
	physics_material_override = current_physics
	debug_label.text = current_physics.resource_name + "\n" + animation.assigned_animation + " - " + sprite.animation + "\ngrounded: " + str(ground_ray.is_colliding()) + "\nactionable: " + str(is_actionable) + "\n" + str("%0.2f" % linear_velocity.y)

func _input(event: InputEvent):
	if (event.is_action_pressed("jump") && is_actionable):
		jump_squat()
	elif (event.is_action("sit") && is_actionable):
		sit()
	elif (event.is_action_pressed("pause")):
		debug_window.visible = !debug_window.visible

func jump_squat():
	if (is_walking || linear_velocity.x == 0):
		_lock_walk_state = true
	animation.play("jump")

func jump():
	if (!ground_ray.is_colliding()):
		return
	_lock_walk_state = false
	sprite.play("jump")
	distance_fallen_label.hide()
	linear_velocity = Vector2(linear_velocity.x, -500)

func fall():
	_lock_walk_state = false
	_falling_height = position.y
	animation.play("fall")
	distance_fallen_label.hide()

func land():
	if (distance_fallen > 10):
		ScoreSourceOfTruth.score = distance_fallen
		distance_fallen_label.show()
		distance_fallen_label.text = str("%0.2f" % distance_fallen," m")
		animation.play("splat")
	else:
		animation.play("land")

func sit():
	animation.play("sit")
	sprite.play("sit") #That is ti avoid the process function directly overriding it

func idle():
	animation.play("idle")


func toggle_music():
	music.stream_paused = !music.stream_paused
