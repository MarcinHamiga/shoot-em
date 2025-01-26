using Godot;

namespace shootem;
public partial class Spawner : Node2D
{
    private PackedScene _enemyScene = GD.Load<PackedScene>("res://scenes/enemy.tscn");

    public void Spawn(Node parent, CharacterBody2D target)
    {
        GD.Print("Spawning enemy");
        Enemy newEnemy = _enemyScene.Instantiate<Enemy>();
        parent.AddChild(newEnemy);

        if (target is Player playerTarget)
        {
            newEnemy.Setup(playerTarget);
            newEnemy.Position = Position;
        }
        else
        {
            GD.PrintErr("Target must be a Player instance");
        }
        
    }
}