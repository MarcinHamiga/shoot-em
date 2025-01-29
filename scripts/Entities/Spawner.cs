using Godot;

namespace Shootemmono.scripts.Entities;
public partial class Spawner : Node2D
{
    private PackedScene _enemyScene = GD.Load<PackedScene>("res://scenes/ghost.tscn");

    public void Spawn(Node parent, CharacterBody2D target)
    {
        // GD.Print("Spawning enemy");
        Ghost newGhost = _enemyScene.Instantiate<Ghost>();
        parent.AddChild(newGhost);

        if (target is Player playerTarget)
        {
            newGhost.Setup(playerTarget);
            newGhost.Position = Position;
        }
        else
        {
            GD.PrintErr("Target must be a Player instance");
        }
        
    }
}