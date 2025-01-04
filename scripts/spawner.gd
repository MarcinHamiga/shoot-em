extends Node2D

var enemy_scene = preload("res://scenes/enemy.tscn")

func spawn(parent: Node, target: CharacterBody2D) -> void:
	print("Spawning enemy")
	var new_enemy: CharacterBody2D = enemy_scene.instantiate()
	parent.add_child(new_enemy)
	new_enemy.setup(target)
	new_enemy.set_position(self.position)
