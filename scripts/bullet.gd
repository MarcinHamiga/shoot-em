extends CharacterBody2D

@export var despawn_in_n_seconds: float = 3
@export var speed = 250

@onready var sprite: Sprite2D = $Sprite
@onready var shape: CollisionShape2D = $Shape
@onready var timer: Timer = $Despawn

var _direction: Vector2
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	self.timer.wait_time = despawn_in_n_seconds


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _physics_process(delta: float) -> void:
	move_and_slide()


func _set_direction(direction: Vector2) -> void:
	self._direction = direction

func _set_sprite() -> void:
	match _direction:
		Vector2(-1, 0):
			self.sprite.flip_h = true
		Vector2(1, 0):
			pass
		Vector2(0, -1):
			self.sprite.rotate(deg_to_rad(-90))
		Vector2(0, 1):
			self.sprite.rotate(deg_to_rad(90))

func _start_timer() -> void:
	self.timer.start()


func setup(direction: Vector2, source_speed: float) -> void:
	self._set_direction(direction)
	self._set_sprite()
	self._start_timer()
	velocity = direction * (speed + source_speed) 


func _on_despawn_timeout() -> void:
	self.queue_free()
