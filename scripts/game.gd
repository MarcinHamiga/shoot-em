extends Node

@onready var spawn_timer: Timer = $SpawnTimer
@onready var entites: Node = $Entities
@onready var player: CharacterBody2D = $Entities/PlayerBody
@onready var score_label: Label = $UI/Score
@onready var spawn_points = $SpawnPoints

var score: int = 0

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	randomize()
	self.player.activate_camera()
	self.spawn_timer.start()
	GameEvents.enemy_killed.connect(self._on_enemy_killed)
	GameEvents.game_over.connect(self._on_game_over)
	
	


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_spawn_timer_timeout() -> void:
	var spawners: Array[Node] = self.spawn_points.get_children()
	var spawner: Node = spawners.pick_random()
	var new_enemy = spawner.spawn(self.entites, self.player)
	self.spawn_timer.start()


func new_game() -> void:
	var children = self.entites.get_children()
	for child in children:
		child.queue_free()
	var player = load("res://scenes/player.tscn").instantiate()
	self.entites.add_child(player)
	player.position = Vector2(256, 256)
	self.spawn_timer.stop()
	self.spawn_timer.start()


func _on_game_over() -> void:
	self.new_game()


func _on_enemy_killed(enemy, bullet) -> void:
	self.score += 10
	self.score_label.text = "Score: %s" % [self.score]
	bullet.queue_free()
	enemy.queue_free()
