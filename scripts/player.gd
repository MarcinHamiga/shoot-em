extends CharacterBody2D

@onready var player_sprite: AnimatedSprite2D = $PlayerSprite
@onready var collision_shape: CollisionShape2D = $Shape
@onready var camera: Camera2D = $Camera
@onready var shoot_timer: Timer = $ShootTimer

@export var fire_rate: float = 0.2
@export var speed: float = 200.0
var facing: Vector2 = Vector2(0, 1)

var can_shoot: bool = true
var is_shooting: bool = false


func _input(event: InputEvent) -> void:
	if event.is_action_pressed("shoot"):
		if self.can_shoot:
			self.shoot()
			self.shoot_timer.start()
		self.is_shooting = true
	if event.is_action_released("shoot"):
		self.is_shooting = false


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	self.shoot_timer.wait_time = fire_rate


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _physics_process(delta: float) -> void:
	get_input()
	move_and_slide()


func get_input() -> void:
	var input_direction: Vector2 = Input.get_vector("walk_left", "walk_right", "walk_up", "walk_down")
	velocity = input_direction * speed
	self.change_anim(input_direction)


func change_anim(input_direction: Vector2) -> void:
	if input_direction == Vector2(0, 0):
		self.stop_walking()
	elif input_direction.x > 0:
		self.facing = input_direction
		if self.player_sprite.animation != "walk_right":
			self.player_sprite.animation = "walk_right"
			self.player_sprite.play()
	elif input_direction.y > 0:
		self.facing = input_direction
		if self.player_sprite.animation != "walk_front":
			self.player_sprite.animation = "walk_front"
			self.player_sprite.play()
	elif input_direction.x < 0:
		self.facing = input_direction
		if self.player_sprite.animation != "walk_left":
			self.player_sprite.animation = "walk_left"
			self.player_sprite.play()
	elif input_direction.y < 0:
		self.facing = input_direction
		if self.player_sprite.animation != "walk_back":
			self.player_sprite.animation = "walk_back"
			self.player_sprite.play()


func stop_walking() -> void:
	match facing:
		Vector2(0, 1):
			self.player_sprite.animation = "stand_front"
		Vector2(-1, 0):
			self.player_sprite.animation = "stand_left"
		Vector2(0, -1):
			self.player_sprite.animation = "stand_back"
		Vector2(1, 0):
			self.player_sprite.animation = "stand_right"
		_:
			self.player_sprite.animation = "stand_front"


func activate_camera() -> void:
	self.camera.make_current()


func shoot() -> void:
	self.can_shoot = false
	var direction: Vector2 = self._get_shooting_direction()
	self.create_bullet(direction)


func _get_shooting_direction() -> Vector2:
	var mouse_pos: Vector2 = get_global_mouse_position()
	var direction: Vector2 = (mouse_pos - self.global_position).normalized()
	return direction


func create_bullet(direction: Vector2) -> void:
	var bullet: CharacterBody2D = load("res://scenes/bullet.tscn").instantiate()
	get_parent().add_child(bullet)
	bullet.setup(direction, self.speed)
	bullet.position = Vector2(self.position) + (Vector2(8, 8) * direction)



func _on_area_body_entered(body: Node2D) -> void:
	print("Enemy entered the player")
	var body_groups = body.get_groups()
	if "enemy" in body_groups:
		GameEvents.emit_signal("game_over")


func _on_shoot_timer_timeout() -> void:
	self.can_shoot = true
	if self.is_shooting:
		self.shoot()
	self.shoot_timer.start()
