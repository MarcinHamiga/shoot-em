extends CharacterBody2D


const SPEED = 95.0
var target: CharacterBody2D


func _physics_process(delta: float) -> void:
	var direction = (target.position - position).normalized()
	velocity = direction * SPEED
	move_and_slide()


func setup(player: CharacterBody2D) -> void:
	self.target = player


func _on_area_body_entered(body: Node2D) -> void:
	var body_groups = body.get_groups()
	if "bullet" in body_groups:
		body.set_physics_process(false)
		self.set_physics_process(false)
		GameEvents.enemy_killed.emit(self, body)
